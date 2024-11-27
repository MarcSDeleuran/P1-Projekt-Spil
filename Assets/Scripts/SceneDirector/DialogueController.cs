using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        private Animator animator;
        private bool isHidden = false;
        private Dictionary<Speaker, SpriteController> sprites;
        public GameObject spritesPrefab;
        public float TextSpeed;
        private Coroutine typingCoroutine;
        public float defaultTextSpeed = 1;

        private enum DialogueState
        {
            PLAYING, SPEEDED_UP, COMPLETED
        }

        private void Awake()
        {
            sprites = new Dictionary<Speaker, SpriteController>();
            animator = GetComponent<Animator>();
            SpeakerNameText = LeftSpeakerNameText;
            animator.SetTrigger("Hide");
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
                DialogueText.text = currentScene.Sentences[sentenceIndex].text;
            state = DialogueState.COMPLETED;
            if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        }

        public void HideBox()
        {
            if (!isHidden)
            {
                animator.SetTrigger("HideBox");
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
                    rightSpeakerName.SetActive(true); // Den her virker ikke af en eller anden grund. Man kan ikke sætte den aktiv selv i editoren, det er vidst noget med animatoren da det virker når man slår den fra
                    leftSpeakerName.SetActive(false);
                    SpeakerNameText = RightSpeakerNameText;
                }
                SpeakerNameText.text = currentScene.Sentences[sentenceIndex].speaker.speakerName;
                leftSpeakerNameBackground.color = currentScene.Sentences[sentenceIndex].speaker.nameColor;
                rightSpeakerNameBackground.color = currentScene.Sentences[sentenceIndex].speaker.nameColor;
                DialogueText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
            }
            else if (SpeakerNameText != null)
                SpeakerNameText.gameObject.SetActive(false);
            ActSpeakers();
            ChangeStats(currentScene.Sentences[sentenceIndex].statChange, currentScene.Sentences[sentenceIndex].changeAmount);
        }

        private void ChangeStats(STATCHANGE statChange, int statAmount)
        {
            switch (statChange)
            {
                case STATCHANGE.NONE: 
                    break;
                case STATCHANGE.SOCIAL:
                    GameManager.Instance.SocialAmount += statAmount;
                    break;
                case STATCHANGE.ACADEMICS:
                    GameManager.Instance.AcademicAmount += statAmount;
                    break;
                case STATCHANGE.STRESS:
                    GameManager.Instance.StressAmount += statAmount;
                    break;
            }
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
                Debug.Log("spritecontroller wasn't null");
                if (action.ActionType == StoryScene.Sentence.Action.Type.DISAPPEAR)
                    return;

                controller.SwitchSprite(action.Speaker.sprites[action.SpriteIndex]);
                Debug.Log(action.SpriteIndex);
            }
        }

        private IEnumerator TypeText(string text)
        {
            DialogueText.text = "";
            state = DialogueState.PLAYING;
            int wordIndex = 0;
            TextSpeed = defaultTextSpeed;

            while (state != DialogueState.COMPLETED)
            {
                DialogueText.text += text[wordIndex];
                yield return new WaitForSeconds(TextSpeed * 0.05f);
                //ska g�res til en public variabel for indstillinger
                if (++wordIndex == text.Length)
                {
                    state = DialogueState.COMPLETED;
                    break;
                }
            }
        }
    }
}

