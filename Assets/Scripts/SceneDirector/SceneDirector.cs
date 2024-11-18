using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SceneDirection
{
    public class SceneDirector : MonoBehaviour
    {
        public GameScene currentScene;
        public DialogueController DC;
        public SpriteSwitcher SC;
        private SceneState state = SceneState.IDLE;
        public OptionSelectionController OSC;
        private enum SceneState
        {
            IDLE, ANIMATE, CHOOSE
        }
        private void Start()
        {
            if (currentScene is StoryScene)
            {
                StoryScene storyScene = (StoryScene)currentScene;
                DC.PlayScene(storyScene);
                SC.SetImage(storyScene.background);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                if (state == SceneState.IDLE && DC.IsCompleted())
                {

                    if (DC.IsLastSentence())
                    {
                        PlayScene((currentScene as StoryScene).nextScene);

                    }
                    else
                        DC.PlayNextSentence();
                }
                else
                {
                    DC.TextSpeed = 0.01f;
                }



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
            currentScene = scene;
            DC.HideBox();
            yield return new WaitForSeconds(1f);
            if (scene is StoryScene)
            {
                StoryScene storyScene = (StoryScene)scene;
                SC.SwitchImage(storyScene.background);
                yield return new WaitForSeconds(1f);
                DC.ClearText();
                DC.ShowBox();
                yield return new WaitForSeconds(1f);
                DC.PlayScene(storyScene);
                state = SceneState.IDLE;
            }
            else
            {
                state = SceneState.CHOOSE;
                OSC.SetupChoose(scene as ChooseScene);
            }
        }

    }
}
