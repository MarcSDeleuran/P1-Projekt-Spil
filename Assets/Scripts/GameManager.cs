using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using SceneDirection;
using UnityEditor.Animations;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startDate;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI academicText;
    [SerializeField] private TextMeshProUGUI socialText;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameSceneUI;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject endSceneUI;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject[] saveFileButtons;
    [SerializeField] private GameObject[] chapterButtons;
    [SerializeField] public bool[] chaptersCompleted;
    public SceneDirector SD;
    public DataHolder DH;
    public FlagManager FM;
    public StatsAndJournal SAJ;
    private int activeSave;
    public static GameManager Instance { get; private set; }
    [Range(0, 200)] public int StressAmount = -1;
    [Range(0, 200)] public int AcademicAmount = -1;
    [Range(0, 200)] public int SocialAmount = -1;
    public string CharacterName;
    public StatChangeAnimator STA;
    public float animationMultiplier;
    public bool MustAssignStats = false;
    public int SaveFileId;   
    public bool day1Achievement = false;
    public bool day2Achievement = false;
    public bool day3Achievement = false;
    public bool day4Achievement = false;
    public bool day5Achievement = false;
    public GameObject day1Trophy;
    public GameObject day2Trophy;
    public GameObject day3Trophy;
    public GameObject day4Trophy;
    public GameObject day5Trophy;
    public int currentChapter;
    

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        chaptersCompleted = new bool[5];
        Application.targetFrameRate = 60;
        UpdateSaveFiles();



        if (!Directory.Exists(Application.dataPath + "/Saves/"))
        { // Opret 'Save' mappe hvis den ikke findes
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        }
    }

    public void ChangeAnimationMultiplier(float f)
    {
        SD.BackgroundSwitcher.GetComponent<Animator>().SetFloat("speedmultiplier", f);
        for (int i = 0; i < SD.DC.spritesPrefab.transform.childCount; i++)
        {
            SD.DC.spritesPrefab.transform.GetChild(i).GetComponent<Animator>().SetFloat("speedmultiplier", f);
        }
        SD.DC.GetComponent<Animator>().SetFloat("speedmultiplier", f);
        SD.OSC.GetComponent<Animator>().SetFloat("speedmultiplier", f);
        SD.SwitchTime = 1 / f;

    }

    #region SaveAndLoadFunctions
    private void UpdateAvailableChapters()
    {
        // Opdater hvilke chapters man kan spille
        DateTime dataCurrent = DateTime.Now;
        for (int i = 0; i < chapterButtons.Length; i++)
        {
            chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().dayText.text = "Unlocks: " + (dataCurrent.Day + i - 1) + ". nov";
            if (dataCurrent.Day >= startDate + i)
            { // Hvis datoen er over startDatoen + ugedage
                chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
            }
            else
            { // Hvis datoen ikke er over startDatoen + ugedage
                chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(true);
            }
        }
    }

    private void UpdateSaveFiles()
    {
        int maxFiles = 3;
        for (int i = 1; i < maxFiles + 1; i++)
        {
            if (File.Exists(Application.dataPath + "/Saves/save" + i + ".txt"))
            { // Hvis man har save-filen (i)
                string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + i + ".txt");
                SaveData saveObject = JsonUtility.FromJson<SaveData>(saveString);

                // Visuelt opdater Save File knapperne
                saveFileButtons[i - 1].GetComponent<SaveFileButtonUI>().UpdateVisual(i, true, saveObject.stressAmount, saveObject.academicAmount, saveObject.socialAmount);
            }
        }
    }

    public void RandomizeValues()
    {
        SaveData saveObject = new SaveData
        { // Opdaterer værdier
            stressAmount = UnityEngine.Random.Range(1, 200),
            academicAmount = UnityEngine.Random.Range(1, 200),
            socialAmount = UnityEngine.Random.Range(1, 200),
        };
        StressAmount = saveObject.stressAmount;
        AcademicAmount = saveObject.academicAmount;
        SocialAmount = saveObject.socialAmount;
        // Undersøg Json fil
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save" + activeSave + ".txt", json);

        // Visuelt opdater tekst og knapper
        stressText.text = "Stress: " + saveObject.stressAmount + "%";
        academicText.text = "Academic: " + saveObject.academicAmount + "%";
        socialText.text = "Social: " + saveObject.socialAmount + "%";
        UpdateSaveFiles();
    }

    public void EnterSaveFile(int saveFileId)
    {
        SaveData saveObject;

        if (File.Exists(Application.dataPath + "/Saves/save" + saveFileId + ".txt"))
        { // Load en Save
            // Undersøg Json fil
            string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + saveFileId + ".txt");
            saveObject = JsonUtility.FromJson<SaveData>(saveString);

                FM.flags = saveObject.flags.ToDictionary(f => Enum.Parse<STORYFLAG>(f.key), f => f.value);

            saveObject.prevScenes.ForEach(scene =>
            {
                SD.history.Add(this.DH.scenes[scene] as StoryScene);
            });
            if (saveObject.prevScenes.Count > 0)
            {
                SD.currentScene = SD.history[SD.history.Count - 1];
                SD.history.RemoveAt(SD.history.Count - 1);
            }
            if (saveObject.sentence != -1)
                SD.DC.SetIndex(saveObject.sentence);
            StressAmount = saveObject.stressAmount;
            AcademicAmount = saveObject.academicAmount;
            SocialAmount = saveObject.socialAmount;
            CharacterName = saveObject.characterName;
            SD.VNACTIVE = true;
            bool[] CC = new bool[5];
            CC[0] = true;
            CC[1] = saveObject.chapterCompletes[1];
            CC[2] = saveObject.chapterCompletes[2];
            CC[3] = saveObject.chapterCompletes[3];
            CC[4] = saveObject.chapterCompletes[4];
            chaptersCompleted = new bool[5];
            chaptersCompleted = CC;
            
            SaveFileId = saveFileId;
        }
        else
        { // Lav en ny Save
          // Sæt default værdier (Skal nok ændres)
            List<int> historyIndices = new List<int>();
            SD.history.ForEach(scene => historyIndices.Add(this.DH.scenes.IndexOf(scene)));

            saveObject = new SaveData
            {

                flags = FM.flags.Select(kvp => new StoryFlag { key = kvp.Key.ToString(), value = kvp.Value }).ToList(),
                sentence = SD.DC.SentenceIndex,
                prevScenes = historyIndices,

                stressAmount = 50,
                academicAmount = 100,
                socialAmount = 100,
                characterName = CharacterName,
                chapterCompletes = new bool[5],
                saveFileId = saveFileId,

            };
            StressAmount = saveObject.stressAmount;
            AcademicAmount = saveObject.academicAmount;
            SocialAmount = saveObject.socialAmount;
            SaveFileId = saveFileId;
            // Konverter til Json fil
            string json = JsonUtility.ToJson(saveObject);
            File.WriteAllText(Application.dataPath + "/Saves/save" + saveFileId + ".txt", json);

            // Visuelt opdater knapper
            UpdateSaveFiles();
        }

        activeSave = saveFileId; // Gem den aktive Save File lokalt

        // Visuelt opdater tekst
        stressText.text = "Stress: " + saveObject.stressAmount;
        academicText.text = "Academic: " + saveObject.academicAmount;
        socialText.text = "Social: " + saveObject.socialAmount;
        UpdateAvailableChapters();
    }

    public void SaveCompletionData()
    {
        SaveData saveObject;
        string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + GameManager.Instance.SaveFileId + ".txt");
        saveObject = JsonUtility.FromJson<SaveData>(saveString);

        saveObject.flags = FM.flags.Select(kvp => new StoryFlag { key = kvp.Key.ToString(), value = kvp.Value }).ToList();
        List<int> historyIndices = new List<int>();
        SD.history.ForEach(scene => historyIndices.Add(this.DH.scenes.IndexOf(scene)));

        saveObject.stressAmount = StressAmount;
        saveObject.academicAmount = AcademicAmount;
        saveObject.socialAmount = SocialAmount;
        saveObject.characterName = CharacterName;
        SD.VNACTIVE = false;
        
        saveObject.chapterCompletes[0] = true;
        saveObject.chapterCompletes[1] = chaptersCompleted[1];
        saveObject.chapterCompletes[2] = chaptersCompleted[2];
        saveObject.chapterCompletes[3] = chaptersCompleted[3];
        saveObject.chapterCompletes[4] = chaptersCompleted[4];
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save" + saveObject.saveFileId + ".txt", json);


    }
    #endregion

    public void EnterChapter(int buttonId)
    {
        DateTime dataCurrent = DateTime.Now; // Få datoen
        
        SD.TimeManagementReward.gameObject.SetActive(false);

        if (buttonId == 0 && dataCurrent.Day >= startDate + 0)
        {  // Hvis datoen er over startDatoen + ugedag

            StartGame(0);
        }
        else if (buttonId == 1 && dataCurrent.Day >= startDate + 1 && SAJ.chapter2Completed)
        {
            StartGame(1);
        }
        else if (buttonId == 2 && dataCurrent.Day >= startDate + 2 && SAJ.chapter3Completed)
        {
            StartGame(2);
        }
        else if (buttonId == 3 && dataCurrent.Day >= startDate + 3 && SAJ.chapter4Completed)
        {
            StartGame(3);
        }
        else if (buttonId == 4 && dataCurrent.Day >= startDate + 4 && SAJ.chapter5Completed)
        {
            StartGame(4);

            mainMenuUI.SetActive(false);
            mainMenuCanvas.SetActive(false);
            gameSceneUI.SetActive(true);
            SD.VNACTIVE = true;
            SD.PlayScene(chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene);
            currentChapter = buttonId + 1;

        }
        else
        { // Hvis datoen ikke er over startDatoen + ugedag
            Debug.LogWarning("You can't enter this Chapter");
        }
    }


    private void StartGame(int buttonId){
       // mainMenuUI.SetActive(false);
       // mainMenuCanvas.SetActive(false);
       // gameSceneUI.SetActive(true);
      //  SD.VNACTIVE = true;
      //  SD.PlayScene(chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene);
        mainMenuUI.SetActive(false);
            mainMenuCanvas.SetActive(false);
            gameSceneUI.SetActive(true);
            SD.VNACTIVE = true;
            SD.PlayScene(chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene);
            currentChapter = buttonId + 1;
    }


    public void MainMenuButton()
    {

        mainMenuUI.SetActive(true);
        gameSceneUI.SetActive(false);
        mainMenuCanvas.SetActive(true);
        endSceneUI.SetActive(false);
        InGameUI.SetActive(true);
        SaveCompletionData();
    }
}
