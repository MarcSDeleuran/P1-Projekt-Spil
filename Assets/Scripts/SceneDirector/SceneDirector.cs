using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    public class SceneDirector : MonoBehaviour
    {
        public GameScene currentScene;
        public DialogueController DC;
        public SpriteSwitcher BackgroundSwitcher;
        private SceneState state = SceneState.IDLE;
        public OptionSelectionController OSC;
        public AudioManager audioManager;

        public List<StoryScene> history;
        public bool VNACTIVE;
        public GameObject NameField;
        public float SwitchTime;
        public GameObject journalUI;
        public EndScreen ES;
        public GameObject mainMenuCanvas;
        public StatsAndJournal SAJ;
        public GameObject InGameUI;
        private enum SceneState
        {
            IDLE, ANIMATE, CHOOSE
        }

        private void Update()
        {
            if (VNACTIVE)
            {
                if (state == SceneState.IDLE)
                {
                    if (currentScene == null) return;

                    if (/*Input.GetKeyDown(KeyCode.Space) ||*/ Input.GetMouseButtonDown(0) && CheckSideOfMouse())
                        if (DC.IsCompleted())
                        {
                            DC.StopTyping();

                            if (DC.IsLastSentence())
                            {
                                if ((currentScene as StoryScene).FinalScene)
                                {
                                    VNACTIVE = false;


                                    string stats = $"Stress: {GameManager.Instance.StressAmount} \n" +
                                        $"Academics: {GameManager.Instance.AcademicAmount} \n" +
                                        $"Social: {GameManager.Instance.SocialAmount}";
                                    ES.Stats.text = stats;

                                    string goals = $"Stress {SAJ.relaxPoints} \n" +
                                        $"social {SAJ.schoolPoints} \n" +
                                        $"school {SAJ.schoolPoints}";
                                    ES.Goals.text = goals;
                                    InGameUI.SetActive(false);
                                    ES.gameObject.SetActive(true);
                                    mainMenuCanvas.SetActive(true);
                                    audioManager.musicSource.Stop();
                                    int c = DC.spritesPrefab.transform.childCount;
                                    for (int i = 0; i < c; i++)
                                    {
                                        Destroy(DC.spritesPrefab.transform.GetChild(i).gameObject);
                                    }
                                    int bc = BackgroundSwitcher.gameObject.transform.childCount;
                                    for ( int i = 0; i < bc; i++)
                                    {
                                        BackgroundSwitcher.transform.GetChild(i).GetComponent<Image>().sprite = null;
                                    }
                                    DC.ResetSprites();
                                    //GameManager.Instance.chaptersCompleted[(currentScene as StoryScene).FinalSceneChapterId] = true;

                                }
                                else
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
                            DC.StopTyping();
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
            if (NameField.GetComponentInChildren<TMPro.TMP_InputField>().text == "")
                return;
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
                yield return new WaitForSeconds(SwitchTime);
            }

            currentScene = scene;
            if (scene is CheckScene)
            {
                CheckScene checkScene = (CheckScene)scene;
                bool reqFound = false;
                foreach (var req in checkScene.Requirement)
                {
                    if (req.AcademicReq != 0)
                    {
                        if (GameManager.Instance.AcademicAmount >= req.AcademicReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }

                    if (req.SocialReq != 0)
                    {
                        if (GameManager.Instance.SocialAmount >= req.SocialReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }

                    if (req.StressReq != 0)
                    {
                        if (GameManager.Instance.StressAmount >= req.StressReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }

                    if (req.AcademicUnderReq != 0)
                    {
                        if (GameManager.Instance.AcademicAmount <= req.AcademicUnderReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }

                    if (req.SocialUnderReq != 0)
                    {
                        if (GameManager.Instance.SocialAmount <= req.SocialUnderReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }

                    if (req.StressUnderReq != 0)
                    {
                        if (GameManager.Instance.StressAmount <= req.StressUnderReq)
                        {
                            PlayScene(req.nextScene);
                            reqFound = true;
                            break;
                        }
                    }
                }
                if (!reqFound)
                    PlayScene(checkScene.DefaultScene);
            }
            if (scene is WriteScene)
            {
                VNACTIVE = false;
                NameField.SetActive(true);
            }
            if (scene is StoryScene)
            {
                StoryScene storyScene = (StoryScene)scene;
                if (storyScene.OpenJournal)
                {
                    journalUI.SetActive(true);
                    GameManager.Instance.MustAssignStats = true;
                    VNACTIVE = false;
                }

                history.Add(storyScene);
                if (BackgroundSwitcher.GetImage() != storyScene.background)
                {
                    BackgroundSwitcher.SwitchImage(storyScene.background);
                    int count = DC.spritesPrefab.transform.childCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (!DC.spritesPrefab.transform.GetChild(i).GetComponent<SpriteController>().hidden)
                        {
                            DC.spritesPrefab.transform.GetChild(i).GetComponent<Animator>().SetTrigger("HideCharacter");
                            DC.spritesPrefab.transform.GetChild(i).GetComponent<SpriteController>().hidden = true;
                        }

                    }
                    yield return new WaitForSeconds(SwitchTime);
                }

                PlayAudio(storyScene.Sentences[0]);


                DC.ClearText();
                DC.ShowBox();
                yield return new WaitForSeconds(SwitchTime);
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
            audioManager.PlayAudio(sentence.Music, sentence.Sound);
        }
        private void PlayAudio(ChooseScene chooseScene)
        {
            audioManager.PlayAudio(chooseScene.Music, chooseScene.Sound);
        }
        private void PlayAudio(WriteScene writeScene)
        {
            audioManager.PlayAudio(writeScene.Music, writeScene.Sound);
        }

    }
}
