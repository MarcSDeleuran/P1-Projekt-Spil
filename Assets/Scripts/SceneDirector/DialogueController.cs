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
        private void Start()
        {
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
