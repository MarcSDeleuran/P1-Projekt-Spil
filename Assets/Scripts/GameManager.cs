using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using SceneDirection;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Audio;

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
    public TextMeshProUGUI journalCurrentDayText;
    public GameObject[] journalMethods;
    public GameObject errorMessagePrefab;
    public Transform errorTextParent;
    public GameObject pauseUI;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    public GameObject creditsUI;
    public GameObject audioManager;
    private bool paused = false;
    public static GameManager Instance { get; private set; }
    //Et singleton skal have static modifier på den reference der hentyder til den. Static betyder at man refere til selve typen og ikke en specifik instans. dvs. sige at der kun er en
    //enkel instans af typen.
    public void Awake()
    {
        //monobehaviour understøtter ikke vores egne constructors, så derfor bruge vi Awake() i stedet
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            //hvis der findes en Instance der referer til noget, der ikke er dette objekt, så skal det slette sig selv
        }
        else
        {
            Instance = this;
            //På den her måde, vil Instance altid hentyde til den rigtige GameManager, uden at Gamemanager behøver at være en static klasse
        }
        chaptersCompleted = new bool[4];
        Application.targetFrameRate = 60;
        UpdateSaveFiles();



        if (!Directory.Exists(Application.dataPath + "/Saves/"))
        { // Opret 'Save' mappe hvis den ikke findes
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        }

        masterVolumeSlider.onValueChanged.AddListener( delegate {
            SetMasterVolume(masterVolumeSlider.value);
        });
        sfxVolumeSlider.onValueChanged.AddListener( delegate {
            SetSFXVolume(sfxVolumeSlider.value);
        });
        musicVolumeSlider.onValueChanged.AddListener( delegate {
            SetMusicVolume(musicVolumeSlider.value);
        });
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            Pause();
        }
    }

    private void Pause(){
        paused = !paused;
        if (paused){
            pauseUI.SetActive(true);
            if (PlayerPrefs.HasKey("MasterVolume")){ // Har spilleren en gemt playerPref (Hvis den ikke har MasterVolume har den ikke nogle af dem)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume"); // Juster sliderne til den gemte værdi
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            } else { // Hvis spilleren ikke har gemt nogle playerPref (Det er første boot-up)
                PlayerPrefs.SetFloat("MasterVolume", 1f);
                PlayerPrefs.SetFloat("SFXVolume", 1f); // Sæt standard values
                PlayerPrefs.SetFloat("MusicVolume", 1f);
            }
        } else {
            pauseUI.SetActive(false);
        }
    }

    private void SetMasterVolume(float value){
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void SetSFXVolume(float value){
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void SetMusicVolume(float value){
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MusicVolume", value);
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
            chapterButtons[i].GetComponent<ChapterButtonUI>().dayText.text = "Unlocks: " + (dataCurrent.Day + i) + ". dec";
            if (dataCurrent.Day >= startDate + i)
            { // Hvis datoen er over startDatoen + ugedage
                if (i != 0){
                    if (chaptersCompleted[i - 1]){
                        chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
                    } else {
                        chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(true);
                    }
                } else {
                    chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
                }
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
            bool[] CC = new bool[4];
            CC[0] = saveObject.chapterCompletes[0];
            CC[1] = saveObject.chapterCompletes[1];
            CC[2] = saveObject.chapterCompletes[2];
            CC[3] = saveObject.chapterCompletes[3];
            chaptersCompleted = new bool[4];
            chaptersCompleted = CC;
            
            SaveFileId = saveFileId;
        }
        else
        { // Lav en ny Save
          // Sæt default værdier (Skal nok ændres)
            List<int> historyIndices = new List<int>();
            SD.history.ForEach(scene => historyIndices.Add(this.DH.scenes.IndexOf(scene)));
            //lavet til at gemme hvilken scene man er på, men vi valgte at gå fra det her, da vi gerne vil have folk spillet et kapitel i en enkel mundbid

            saveObject = new SaveData
            {
                // SaveData er et struct, der indeholder alle de værdier vi ønsker at gemme. Et struct er ligesom en klasse, men den har mindre funktionalitet. de reele forskelle er 
                //går udover hva de forventer i ved til eksamen, bare ved at structs er mere effektive, hvis man bare skal give nogle værdier videre
                flags = FM.flags.Select(kvp => new StoryFlag { key = kvp.Key.ToString(), value = kvp.Value }).ToList(),
                //fordi vores flags er i et dictionary, og man kan ikke gemme et dictionary på en json-fil, så laver vi en liste med "StoryFlag", som er et struct, der indeholder
                // en string og en bool værdi, som tilsvare key og value'et på hvert item i dictionary'et
                sentence = SD.DC.SentenceIndex,
                prevScenes = historyIndices,

                stressAmount = 50,
                academicAmount = 50,
                socialAmount = 50,
                characterName = CharacterName,
                chapterCompletes = new bool[4],
                saveFileId = saveFileId,

                
            };
            chaptersCompleted = saveObject.chapterCompletes;
            StressAmount = saveObject.stressAmount;
            AcademicAmount = saveObject.academicAmount;
            SocialAmount = saveObject.socialAmount;
            SaveFileId = saveFileId;
            // Konverter til Json fil. Json fil er et tekst format, der er let for mennesker at læse og skrive, og let for computer parse og generer. parsing er at bryde data ned og ændre til
            //et andet format, som computeren bedre kan arbejde med. Det det vi gør nedenunder, når vi skriver txt.fil ud fra vores json-objekt
            string json = JsonUtility.ToJson(saveObject);
            File.WriteAllText(Application.dataPath + "/Saves/save" + saveFileId + ".txt", json);
            //skriver teksten i et txt-fil på den angivne directory

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
        
        saveObject.chapterCompletes[0] = chaptersCompleted[0];
        saveObject.chapterCompletes[1] = chaptersCompleted[1];
        saveObject.chapterCompletes[2] = chaptersCompleted[2];
        saveObject.chapterCompletes[3] = chaptersCompleted[3];
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save" + saveObject.saveFileId + ".txt", json);


    }
    #endregion

    public void EnterChapter(int buttonId)
    {
        DateTime dataCurrent = DateTime.Now; // Få datoen
        
        SD.TimeManagementReward.gameObject.SetActive(false);

        if (buttonId == 0 && dataCurrent.Day >= startDate && !chaptersCompleted[0])
        {  // Hvis datoen er over startDatoen + ugedag

            StartGame(0);
        }
        else if (buttonId == 4 && dataCurrent.Day >= startDate + buttonId && chaptersCompleted[buttonId - 1])
        {
            StartGame(buttonId);
        }
        else if (dataCurrent.Day >= startDate + buttonId && chaptersCompleted[buttonId - 1] && !chaptersCompleted[buttonId])
        {
            StartGame(buttonId);
        }
        else
        { // Hvis datoen ikke er over startDatoen + ugedag
            Instantiate(errorMessagePrefab, errorTextParent);
        }
    }


    private void StartGame(int buttonId){
        mainMenuUI.SetActive(false);
        mainMenuCanvas.SetActive(false);
        gameSceneUI.SetActive(true);
        SD.VNACTIVE = true;
        SD.PlayScene(chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene);
        currentChapter = buttonId + 1;
        foreach (GameObject journalMethod in journalMethods){
            journalMethod.SetActive(false);
        }
        journalMethods[buttonId].SetActive(true);
        switch (currentChapter){
            case 1:
                journalCurrentDayText.text = "Monday";
                break;
            case 2:
                journalCurrentDayText.text = "Tuesday";
                break;
            case 3:
                journalCurrentDayText.text = "Wednesday";
                break;
            case 4:
                journalCurrentDayText.text = "Thursday";
                break;
            case 5:
                journalCurrentDayText.text = "Friday";
                break;
        }
    }

    public void MainMenuButton()
    {
        mainMenuUI.SetActive(true);
        gameSceneUI.SetActive(false);
        mainMenuCanvas.SetActive(true);
        endSceneUI.SetActive(false);
        InGameUI.SetActive(true);
        SaveCompletionData();
        UpdateAvailableChapters();
        SAJ.ChangeAllowed = true;
        paused = false;
        pauseUI.SetActive(false);
        SD.VNACTIVE = false;
    }

    public void ContinueButton(){
        if (currentChapter == 5){
            creditsUI.SetActive(true);
            audioManager.SetActive(false);
        } else {
            mainMenuUI.SetActive(true);
            gameSceneUI.SetActive(false);
            mainMenuCanvas.SetActive(true);
            endSceneUI.SetActive(false);
            InGameUI.SetActive(true);
            SaveCompletionData();
            UpdateAvailableChapters();
            SAJ.ChangeAllowed = true;
            paused = false;
            pauseUI.SetActive(false);
            SD.VNACTIVE = false;
        }
    }

    public void OpenLink(){
        Application.OpenURL("https://www.survey-xact.dk/LinkCollector?key=J6AM2TAQJ69J");
    }

    public void Quit(){
        Application.Quit();
    }
}
