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
            animator.SetTrigger("ShowBox");
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
            animator.SetTrigger("ShowBox");
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
            else if (stress != 0 && stress < 0)
                STA.ShowStatChange("Stress", false);

            GameManager.Instance.AcademicAmount += academic;
            if (GameManager.Instance.AcademicAmount < 0)
                GameManager.Instance.AcademicAmount = 0;
            if (GameManager.Instance.AcademicAmount > 200)
                GameManager.Instance.AcademicAmount = 200;

            if (academic > 0 && academic != 0)
                STA.ShowStatChange("Academics", true);
            else if (academic != 0 && academic <0)
                STA.ShowStatChange("Academics", false);

            GameManager.Instance.SocialAmount += social;
            if (GameManager.Instance.SocialAmount < 0)
                GameManager.Instance.SocialAmount = 0;
            if (GameManager.Instance.SocialAmount > 200)
                GameManager.Instance.SocialAmount = 200;

            if (social > 0 && social != 0)
                STA.ShowStatChange("Social", true);
            else if (social != 0 && social < 0)
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
            switch (action.ActionType)
            {
                case StoryScene.Sentence.Action.Type.APPEAR:
                    if (!sprites.ContainsKey(action.Speaker))
                    {
                        controller = Instantiate(action.Speaker.prefab.gameObject, spritesPrefab.transform).GetComponent<SpriteController>();
                        sprites.Add(action.Speaker, controller);
                    }
                    else
                    {
                        controller = sprites[action.Speaker];
                    }
                    controller.Setup(action.Speaker.sprites[action.SpriteIndex]);
                    controller.Show(action.Coords);
                    return;
                case StoryScene.Sentence.Action.Type.MOVE:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        controller.Move(action.Coords, action.MoveSpeed);
                        controller = sprites[action.Speaker];
                    }
                    break;
                case StoryScene.Sentence.Action.Type.DISAPPEAR:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        Debug.Log(action.SpriteIndex);
                        controller.Hide();
                    }
                    break;
                case StoryScene.Sentence.Action.Type.FLIP:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        controller.gameObject.transform.Rotate(0, 180, 0);
                    }
                    break;
                case StoryScene.Sentence.Action.Type.NONE:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
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
            DialogueText.text = "";
            state = DialogueState.PLAYING;
            int wordIndex = 0;
            bool playOnNextLetter = false;

            while (state != DialogueState.COMPLETED)
            {
                DialogueText.text += text[wordIndex];
                if (playOnNextLetter){ // Bare sådan det kun er hvert andet bogstav, ellers bliver det for meget
                    source.PlayOneShot(sound);
                    playOnNextLetter = false;
                } else {
                    playOnNextLetter = true;
                }
                yield return new WaitForSeconds(TextSpeed * 0.05f);
                if (++wordIndex == text.Length)
                {
                    state = DialogueState.COMPLETED;
                    break;
                }
            }
        }
    }
}

