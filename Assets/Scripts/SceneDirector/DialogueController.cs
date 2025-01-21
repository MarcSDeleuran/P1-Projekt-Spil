using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace SceneDirection
{
    public class DialogueController : MonoBehaviour
    {
        public TextMeshProUGUI DialogueText;
        private TextMeshProUGUI SpeakerNameText;
        public GameObject leftSpeakerName;
        public GameObject rightSpeakerName;
        public TextMeshProUGUI LeftSpeakerNameText;
        public TextMeshProUGUI RightSpeakerNameText;
        public Image leftSpeakerNameBackground;
        public Image rightSpeakerNameBackground;
        private int sentenceIndex = -1;
        public int SentenceIndex { get { return sentenceIndex; } }
        public StoryScene currentScene;
        private DialogueState state = DialogueState.COMPLETED;
        public Animator animator;
        private bool isHidden = false;
        private Dictionary<Speaker, SpriteController> sprites;
        public GameObject spritesPrefab;
        public float TextSpeed;
        private Coroutine typingCoroutine;
        public AudioClip sound;
        public AudioSource source;

        private enum DialogueState
        {
            PLAYING, SPEEDED_UP, COMPLETED
        }

        private void Awake()
        {
            sprites = new Dictionary<Speaker, SpriteController>();
            animator = GetComponent<Animator>();
            SpeakerNameText = LeftSpeakerNameText;
            animator.SetTrigger("Show");
        }
        public void ResetSprites()
        {
            sprites = new Dictionary<Speaker, SpriteController>();
        }
        public void SetIndex(int i)
        {
            sentenceIndex = i;
        }

        #region Bools/Getters
        public bool IsLastSentence()
        {
            return sentenceIndex + 1 == currentScene.Sentences.Count;
        }
        public bool IsCompleted()
        {
            return state == DialogueState.COMPLETED || state == DialogueState.SPEEDED_UP;
        }
        #endregion

        public void SpeedUp()
        {
            state = DialogueState.SPEEDED_UP;
            TextSpeed = 0.25f;
        }

        public void StopTyping()
        {
            if (sentenceIndex != -1)
                DialogueText.text = currentScene.Sentences[sentenceIndex].text.Replace("[PlayerName]", GameManager.Instance.CharacterName);
            state = DialogueState.COMPLETED;
            if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        }

        public void HideBox()
        {
            if (!isHidden)
            {
                rightSpeakerName.SetActive(false);
                leftSpeakerName.SetActive(false);
                animator.SetTrigger("Hide");
                isHidden = true;
            }
        }

        public void ShowBox()
        {
            animator.SetTrigger("Show");
            isHidden = false;
        }

        public void ClearText()
        {
            DialogueText.text = "";
        }

        public void PlayScene(StoryScene scene)
        {
            currentScene = scene;
            sentenceIndex = -1;
            PlayNextSentence();
        }

        public void PlayNextSentence()
        {
            string text = currentScene.Sentences[++sentenceIndex].text;
            string newText = text.Replace("[PlayerName]", GameManager.Instance.CharacterName);
            typingCoroutine = StartCoroutine(TypeText(newText));
            if (currentScene.Sentences[sentenceIndex].speaker != null)
            {
                SpeakerNameText.gameObject.SetActive(true);
                if (currentScene.Sentences[sentenceIndex].speaker.LeftSide)
                {
                    rightSpeakerName.SetActive(false);
                    leftSpeakerName.SetActive(true);
                    SpeakerNameText = LeftSpeakerNameText;
                }
                else
                {
                    rightSpeakerName.SetActive(true);
                    leftSpeakerName.SetActive(false);
                    SpeakerNameText = RightSpeakerNameText;
                }
                SpeakerNameText.text = currentScene.Sentences[sentenceIndex].speaker.speakerName;
                leftSpeakerNameBackground.color = currentScene.Sentences[sentenceIndex].speaker.nameColor;
                rightSpeakerNameBackground.color = currentScene.Sentences[sentenceIndex].speaker.nameColor;
                DialogueText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
            }
            else 
            {
                SpeakerNameText.gameObject.SetActive(false);
                DialogueText.color = Color.white;
                rightSpeakerName.SetActive(false);
                leftSpeakerName.SetActive(false);
            }

            ActSpeakers();
            ChangeStats(currentScene.Sentences[sentenceIndex].SocialChange, currentScene.Sentences[sentenceIndex].AcademicChange, currentScene.Sentences[sentenceIndex].StressChange);
        }

        private void ChangeStats(int social, int academic, int stress)
        {
            StatChangeAnimator STA = GameManager.Instance.STA;
            GameManager.Instance.StressAmount += stress;
            if (GameManager.Instance.StressAmount < 0)
                GameManager.Instance.StressAmount = 0;
            if (GameManager.Instance.StressAmount > 200)
                GameManager.Instance.StressAmount = 200;

            if (stress > 0 && stress != 0)
                STA.ShowStatChange("Stress", true);
            else if (stress != 0)
                STA.ShowStatChange("Stress", false);

            GameManager.Instance.AcademicAmount += academic;
            if (GameManager.Instance.AcademicAmount < 0)
                GameManager.Instance.AcademicAmount = 0;
            if (GameManager.Instance.AcademicAmount > 200)
                GameManager.Instance.AcademicAmount = 200;

            if (academic > 0 && academic != 0)
                STA.ShowStatChange("Academics", true);
            else if (academic != 0)
                STA.ShowStatChange("Academics", false);

            GameManager.Instance.SocialAmount += social;
            if (GameManager.Instance.SocialAmount < 0)
                GameManager.Instance.SocialAmount = 0;
            if (GameManager.Instance.SocialAmount > 200)
                GameManager.Instance.SocialAmount = 200;

            if (social > 0 && social != 0)
                STA.ShowStatChange("Social", true);
            else if (social != 0)
                STA.ShowStatChange("Social", false);
        }
            
        private void ActSpeakers()
        {
            List<StoryScene.Sentence.Action> actions = currentScene.Sentences[sentenceIndex].Actions;
            for (int i = 0; i < actions.Count; i++)
            {
                HandleSpeakerAction(actions[i]);
            }
        }

        private void HandleSpeakerAction(StoryScene.Sentence.Action action)
        {
            SpriteController controller = null;
            //laver en ny lokal spritecontroller, og sætter værdien til at være null. det er lavet udenfor switch casen, sådan at vi ikke behøver at instantiere det i hvert case, og sådan man
            //man kan bruge den udenfor den bagefter. SpriteController er den klasse, der er ansvarlig for spritesne på vores karaktere. den ka skifte sprite, flytte sig osv.
            switch (action.ActionType)
            {
                case StoryScene.Sentence.Action.Type.APPEAR:
                    if (!sprites.ContainsKey(action.Speaker))
                    {
                        //sprites er et dictionary. en dictionary fungere som en liste med 2 værdier på hver plads i stedet for en. dvs. en Key og en Value, og det er lavet sådan
                        //let at finde en value ud fra en key. ligesom det er lidt at finde et bestemt sted i en bog, når du kender sidetallet. Key'et i det her tilfælde er en speaker, så
                        //den kigger om sprites-dictionary'et allerede indeholder den speaker, der hænger på den nuværende action.
                        controller = Instantiate(action.Speaker.prefab.gameObject, spritesPrefab.transform).GetComponent<SpriteController>();
                        sprites.Add(action.Speaker, controller);
                        //hvis den ikke indeholder det, så instantiere den en nyt gameobject ud fra prefabet charactersprites. det det "action.speaker.PREFAB.gameObject" hentyder til. bagefter 
                        //finder den komponentet SpriteController i det nye objekt, og sætter vores lokale  værdi "controller" til at være den. til sidst tilføjer den controller-værdien til vores
                        //sprites dictionary, hvor key'et er den speaker, der tilsvarer den nuværende action. Så næste gang der er en action med den her speaker, ka den hurtigt finde den tilsvarende
                        //spritecontroller, den ska ha fat i
                    }
                    else
                    {
                        controller = sprites[action.Speaker];
                    }
                    controller.Setup(action.Speaker.sprites[action.SpriteIndex]);
                    controller.Show(action.Coords);
                    //setup giver den det rigtige sprite, og show får den til at fade ind på de koordinater den er blevet givet
                    return;
                    //vi returner her fordi vi ikke skal køre koden efter switchcasen i det her tilfælde
                case StoryScene.Sentence.Action.Type.MOVE:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        controller.Move(action.Coords, action.MoveSpeed);
                        //move får spritet til at flytte sig imod de nye koordinater, med den hastighed der bliver givet i movespeed
                        controller = sprites[action.Speaker];
                    }
                    break;
                case StoryScene.Sentence.Action.Type.DISAPPEAR:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        Debug.Log(action.SpriteIndex);
                        controller.Hide();
                        //hide får spritet til at fade ud
                    }
                    break;
                case StoryScene.Sentence.Action.Type.FLIP:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        controller.gameObject.transform.Rotate(0, 180, 0);
                        //flip får spritet til at vende den anden retning
                    }
                    break;
                case StoryScene.Sentence.Action.Type.NONE:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        //sjovt nok ingen handling her. er her hvis man vil ændre den nuværende sprite og intet andet.
                    }
                    break;
            }

            if (controller != null)
            {
                if (action.ActionType == StoryScene.Sentence.Action.Type.DISAPPEAR)
                    return;

                controller.SwitchSprite(action.Speaker.sprites[action.SpriteIndex]);
            }
        }

        private IEnumerator TypeText(string text)
        {
            //for at lave en coroutine, skal metoden returnere IEnumerator
            DialogueText.text = "";
            //sætter teksten i vores dialogue-box til at være tom
            state = DialogueState.PLAYING;
            //dette enum bliver brugt, for at se hvor lang tid det skal blive ved med at skrive, men også sådan andre klasser kan se om DC'en er igang.
            int wordIndex = 0;
            //index for hvor langt vi er i den nuværende sentence's tekst
            bool playOnNextLetter = false;
            while (state != DialogueState.COMPLETED)
            {
                DialogueText.text += text[wordIndex];
                //hvis wordindex nu var 23, så ville den finde det 23. bogstav i texten og tilføje det til vores text i dialogue boksen, og så plusser vi wordindex med en og køre det igen. 
                if (playOnNextLetter){ // Bare sådan det kun er hvert andet bogstav, ellers bliver det for meget
                    source.PlayOneShot(sound);
                    playOnNextLetter = false;
                } else {
                    playOnNextLetter = true;
                }
                yield return new WaitForSeconds(TextSpeed * 0.05f);
                //dette sætter coroutinen på pause, så det går en smule tid imellem hvert bogstav bliver skrevet.
                if (++wordIndex == text.Length)
                {
                    state = DialogueState.COMPLETED;
                    break;
                    //når vi har skrevet alle de bogstaver der i vores tekst, sætter vi vores dialoguestate til at være complete, så while-loopet holder og coroutinen dør
                }
            }
        }
    }
}

