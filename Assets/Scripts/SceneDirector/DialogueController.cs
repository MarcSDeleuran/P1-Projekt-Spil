using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

namespace SceneDirection
{
  
    public class DialogueController : MonoBehaviour
    {
        public TextMeshProUGUI DialogueText;
        private TextMeshProUGUI SpeakerNameText;
        public TextMeshProUGUI LeftSpeakerNameText;
        public TextMeshProUGUI RightSpeakerNameText;
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
            DialogueText.text = currentScene.Sentences[sentenceIndex].text;
            state = DialogueState.COMPLETED;
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
            text.Replace("NAME", "bob");
            typingCoroutine = StartCoroutine(TypeText(text));
            if (currentScene.Sentences[sentenceIndex].speaker != null)
            {
                SpeakerNameText.gameObject.SetActive(true);
                if (currentScene.Sentences[sentenceIndex].speaker.LeftSide)
                {
                    RightSpeakerNameText.gameObject.SetActive(false);
                    LeftSpeakerNameText.gameObject.SetActive(true);
                    SpeakerNameText = LeftSpeakerNameText;
                }
                else
                {
                    RightSpeakerNameText.gameObject.SetActive(true);
                    LeftSpeakerNameText.gameObject.SetActive(false);
                    SpeakerNameText = RightSpeakerNameText;
                }
                SpeakerNameText.text = currentScene.Sentences[sentenceIndex].speaker.speakerName;
                SpeakerNameText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
                DialogueText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
            }
            else if (SpeakerNameText != null)
                SpeakerNameText.gameObject.SetActive(false);
            ActSpeakers();
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
                //ska gøres til en public variabel for indstillinger
                if (++wordIndex == text.Length)
                {
                    state = DialogueState.COMPLETED;
                    break;
                }

            }
        }
    }


}

