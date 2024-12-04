using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
        public Image TimeManagementReward;
        public Image socialReward;
        public Image academicReward;
        public Image stressReward;
        public Sprite goldMedal;
        public Sprite silverMedal;
        public Sprite bronzeMedal;
        public Sprite socialGoal;
        public Sprite academicGoal;
        public Sprite stressGoal;
        public Sprite notAchievedGoal;
        public bool academicGoalReached = false;
        public bool socialGoalReached = false;
        public bool stressGoalReached = false;
        public TextMeshProUGUI rewardText;
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
                                    GameManager.Instance.chaptersCompleted[(currentScene as StoryScene).FinalSceneChapterId] = true;

                                    int stressAmoutLower = SAJ.relaxPoints - 10;
                                    int stressAmoutUpper = SAJ.relaxPoints + 10;
                                    int socialAmountLower = SAJ.socialPoints - 10;
                                    int socialAmountUpper = SAJ.socialPoints + 10;
                                    int schoolAmountLower = SAJ.schoolPoints - 10;
                                    int schoolAmountUpper = SAJ.schoolPoints + 10;

                                    if (GameManager.Instance.StressAmount <= stressAmoutUpper)
                                    {
                                        stressGoalReached = true;
                                        

                                        
                                        Debug.Log("You have achieved something");
                                        
                                        
                                    }

                                   
                                    
                                    if (GameManager.Instance.SocialAmount >= socialAmountLower)
                                    {
                                        
                                        socialGoalReached = true;
                                        
                                        

                                        
                                        Debug.Log("You have achieved something");
                                        TimeManagementReward.gameObject.SetActive(true);
                                        
                                    }
                                    
                                   
                                    if (GameManager.Instance.AcademicAmount >= schoolAmountLower)
                                    {
                                        academicGoalReached = true;
                                        
                                        

                                        
                                        Debug.Log("You have achieved something");
                                        TimeManagementReward.gameObject.SetActive(true);
                                        
                                    }

                                    else
                                    {
                                        rewardText.text = "You didn't reach your targeted stress goal for today. You might want to change it up for tommorrow. Try setting a more realistic goal or prioritize you choices";
                                    }
                                    
                                    
                                    
                                    
                                    if(academicGoalReached && socialGoalReached && stressGoalReached)
                                    {
                                        rewardText.text = "Congratulations! You have reached your targeted stress goal for today. Now that you know the goal is reasonable, you might want to change it up for tommorrow.";
                                        TimeManagementReward.sprite = goldMedal;
                                        academicReward.sprite = academicGoal;
                                        socialReward.sprite = socialGoal;
                                        stressReward.sprite = stressGoal;
                                        
                                    }
                                    else if (academicGoalReached)
                                    {
                                        TimeManagementReward.sprite = bronzeMedal;
                                        rewardText.text = "Today you have achieved your academic goal. But you seem to have left the social aspect unattended. As it can be seen on your stress level, this might be something to change up tommorrow.";
                                        academicReward.sprite = academicGoal;
                                        stressReward.sprite = notAchievedGoal;
                                        socialReward.sprite = notAchievedGoal;
                                    }
                                    else if (stressGoalReached)
                                    {
                                        TimeManagementReward.sprite = bronzeMedal;
                                        stressReward.sprite = stressGoal;
                                        socialReward.sprite = notAchievedGoal;
                                        academicReward.sprite = notAchievedGoal;
                                        rewardText.text = "Today you have achieved your stress goal. But it has come on the cost of your social and academic life. It is important to remember to balance your life";
                                    }
                                    else if (socialGoalReached)
                                    {
                                        TimeManagementReward.sprite = bronzeMedal;
                                        socialReward.sprite = socialGoal;
                                        academicReward.sprite = notAchievedGoal;
                                        stressReward.sprite = notAchievedGoal;
                                        rewardText.text = "Today you have achieved your social goal. But is has come on the cost of your academic performance. As it can be seen on your stress level, this have had a negative effect on your stress. The social aspect is important, but you need to prioritise your academic life aswell";
                                    }
                                    else if (academicGoalReached && socialGoalReached)
                                    {
                                        TimeManagementReward.sprite = silverMedal;
                                        socialReward.sprite = socialGoal;
                                        academicReward.sprite = academicGoal;
                                        stressReward.sprite = notAchievedGoal;
                                        rewardText.text = "Today you have achieved your academic and social goal. That is quite impressive. However you seem to have overestimated the effects on the stress level. Reducing stress is something that takes time and consistency. Keep doing what you are doing and you will see the stress bar fall";
                                    }
                                    else if (academicGoalReached && stressGoalReached)
                                    {
                                        TimeManagementReward.sprite = silverMedal;
                                        stressReward.sprite = stressGoal;
                                        academicReward.sprite = academicGoal;
                                        socialReward.sprite = notAchievedGoal;
                                        rewardText.text = "Today you have achieved your academic and stress goals for today. That is very impressive. However, you need to remember that your social life also is important. It could have consequenses for your stress levels going forward...";
                                    }
                                    else if (socialGoalReached && stressGoalReached)
                                    {
                                        TimeManagementReward.sprite = silverMedal;
                                        stressReward.sprite = stressGoal;
                                        socialReward.sprite = socialGoal;
                                        academicReward.sprite = notAchievedGoal;
                                        rewardText.text = "Today you have achieved you social and stress goals. However you seem to be behind on your academic level. Only prioritizing your social life may reduce stress for awhile. But being behind in your academic life will eventuelly catch up to you.";
                                    }
                                    else
                                    {
                                        TimeManagementReward.sprite = notAchievedGoal;
                                        rewardText.text = "You have not reached any of your goals today. Try setting more realistic expectations for yourself";
                                    }
                                    
                                    switch (GameManager.Instance.currentChapter)
                                    {
                                        case 1:
                                            GameManager.Instance.day1Achievement = true;
                                            GameManager.Instance.day1Trophy.GetComponent<Image>().sprite = TimeManagementReward.sprite;
                                            GameManager.Instance.day1Trophy.SetActive(true);
                                                
                                            break;
                                        case 2:
                                            GameManager.Instance.day2Achievement = true;
                                            GameManager.Instance.day2Trophy.GetComponent<Image>().sprite = TimeManagementReward.sprite;
                                            GameManager.Instance.day2Trophy.SetActive(true);
                                            break;
                                        case 3:
                                            GameManager.Instance.day3Achievement = true;
                                            GameManager.Instance.day3Trophy.GetComponent<Image>().sprite = TimeManagementReward.sprite;
                                            GameManager.Instance.day3Trophy.SetActive(true);
                                            break;
                                        case 4:
                                            GameManager.Instance.day4Achievement = true;
                                            GameManager.Instance.day4Trophy.GetComponent<Image>().sprite = TimeManagementReward.sprite;
                                            GameManager.Instance.day4Trophy.SetActive(true);
                                            break;
                                        case 5:
                                            GameManager.Instance.day5Achievement = true;
                                            GameManager.Instance.day5Trophy.GetComponent<Image>().sprite = TimeManagementReward.sprite;
                                            GameManager.Instance.day5Trophy.SetActive(true);
                                            break;
                                    }


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
