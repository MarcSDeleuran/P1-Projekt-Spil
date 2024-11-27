using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static SceneDirection.StoryScene;

namespace SceneDirection
{
    public class SceneDirector : MonoBehaviour
    {
        public GameScene currentScene;
        public DialogueController DC;
        public SpriteSwitcher BackgroundSwitcher;
        private SceneState state = SceneState.IDLE;
        public OptionSelectionController OSC;
        public AudioManager AudioManager;

        public List<StoryScene> history;
        public bool VNACTIVE;
        public GameObject NameField;

        private enum SceneState
        {
            IDLE, ANIMATE, CHOOSE
        }
        private void Start()
        {
            //if (currentScene is StoryScene)
            //{
            //    StoryScene storyScene = (StoryScene)currentScene;
            //    history.Add(storyScene);
            //    DC.PlayScene(storyScene);
            //    BackgroundSwitcher.SetImage(storyScene.background);
            //    if (storyScene.Sentences.Count != 0)
            //        PlayAudio(storyScene.Sentences[0]);
            //}
        }

        private void Update()
        {
            if (VNACTIVE)
            {
                if (state == SceneState.IDLE)
                {
                    if (currentScene == null) return;

                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && CheckSideOfMouse())
                        if (DC.IsCompleted())
                        {
                            DC.StopTyping();

                            if (DC.IsLastSentence())
                            {
                                PlayScene((currentScene as StoryScene).nextScene);

                            }
                            else
                            {
                                DC.PlayNextSentence();
                                PlayAudio((currentScene as StoryScene).Sentences[DC.SentenceIndex]);
                            }


                        }
                        else
                        {
                            DC.SpeedUp();
                        }
                }
            }


        }
        public bool CheckSideOfMouse()
        {
            Vector3 mousePosition = Input.mousePosition;

            float screenWidth = Screen.width;

            if (mousePosition.x > screenWidth / 2)
                return true;
            else return false;
        }
        public void SaveName()
        {
            GameManager.Instance.CharacterName = NameField.GetComponentInChildren<TMPro.TMP_InputField>().text;
            VNACTIVE = true;
            WriteScene ws = currentScene as WriteScene;
            PlayScene(ws.NextScene);
            NameField.SetActive(false);
            

        }
        public void PlayScene(GameScene scene)
        {
            StartCoroutine(SwitchScene(scene));
        }
        private IEnumerator SwitchScene(GameScene scene)
        {
            if (scene == null)
            {
                Debug.Log("nextScene Not available");
                yield break;
            }

            state = SceneState.ANIMATE;
            if (currentScene != null)
            {
                DC.HideBox();
                yield return new WaitForSeconds(1f);
            }
                
            currentScene = scene;
            if (scene is WriteScene)
            {
                VNACTIVE = false;
                NameField.SetActive(true);
            }
            if (scene is StoryScene)
            {
                StoryScene storyScene = (StoryScene)scene;
                history.Add(storyScene);
                if (BackgroundSwitcher.GetImage() != storyScene.background)
                {
                    BackgroundSwitcher.SwitchImage(storyScene.background);
                    yield return new WaitForSeconds(1f);
                }

                PlayAudio(storyScene.Sentences[0]);


                DC.ClearText();
                DC.ShowBox();
                yield return new WaitForSeconds(1f);
                DC.PlayScene(storyScene);
                state = SceneState.IDLE;
            }
            else if (scene is ChooseScene)
            {
                state = SceneState.CHOOSE;
                PlayAudio(scene as ChooseScene);
                OSC.SetupChoose(scene as ChooseScene);
            }
        }

        private void PlayAudio(StoryScene.Sentence sentence)
        {
            AudioManager.PlayAudio(sentence.Music, sentence.Sound);
        }
        private void PlayAudio(ChooseScene chooseScene)
        {
            AudioManager.PlayAudio(chooseScene.Music, chooseScene.Sound);
        }
        private void PlayAudio(WriteScene writeScene)
        {
            AudioManager.PlayAudio(writeScene.Music, writeScene.Sound);
        }

    }
}
