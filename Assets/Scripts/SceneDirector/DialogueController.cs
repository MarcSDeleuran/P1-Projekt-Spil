using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SceneDirection
{
    public class DialogueController : MonoBehaviour
    {
        public TextMeshProUGUI DialogueText;
        public TextMeshProUGUI SpeakerNameText;

        private int sentenceIndex = -1;
        public StoryScene currentScene;
        private DialogueState state = DialogueState.COMPLETED;
        private Animator animator;
        private bool isHidden = false;  
        private Dictionary<Speaker, SpriteController> sprites;
        public GameObject spritesPrefab;
        private void Awake()
        {
            sprites = new Dictionary<Speaker, SpriteController>();
            animator = GetComponent<Animator>();
        }
        private enum DialogueState
        {
            PLAYING, COMPLETED
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
            StartCoroutine(TypeText(currentScene.Sentences[++sentenceIndex].text));
            SpeakerNameText.text = currentScene.Sentences[sentenceIndex].speaker.speakerName;
            SpeakerNameText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
            DialogueText.color = currentScene.Sentences[sentenceIndex].speaker.textColor;
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
                    }
                    break;
                case StoryScene.Sentence.Action.Type.DISAPPEAR:
                    if (sprites.ContainsKey(action.Speaker))
                    {
                        controller = sprites[action.Speaker];
                        controller.Hide();
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
                Debug.Log("spritecontroller was null");
                controller.SwitchSprite(action.Speaker.sprites[action.SpriteIndex]);
            }
        }

        public bool IsLastSentence()
        {
            return sentenceIndex + 1 == currentScene.Sentences.Count;
        }
        public bool IsCompleted()
        {
            return state == DialogueState.COMPLETED;
        }
        private IEnumerator TypeText(string text)
        {
            DialogueText.text = "";
            state = DialogueState.PLAYING;
            int wordIndex = 0;

            while (state != DialogueState.COMPLETED)
            {
                DialogueText.text += text[wordIndex];
                yield return new WaitForSeconds(0.05f);
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
