using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    [SerializeField] private int startDate;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI academicText;
    [SerializeField] private TextMeshProUGUI socialText;
    [SerializeField] private GameObject[] saveFileButtons;
    [SerializeField] private GameObject[] chapterButtons;
    public SceneDirection.SceneDirector SD;
    public DataHolder DH;
    public FlagManager FM;
    private int activeSave;

    public void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Destroy(this);
        }
        else 
            Instance = this;

        Application.targetFrameRate = 60;
        UpdateSaveFiles();

        UpdateAvailableChapters();

        if (!Directory.Exists(Application.dataPath + "/Saves/"))
        { // Opret 'Save' mappe hvis den ikke findes
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        }
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
        { // Opdaterer v�rdier
            stressAmount = UnityEngine.Random.Range(1, 100),
            academicAmount = UnityEngine.Random.Range(1, 100),
            socialAmount = UnityEngine.Random.Range(1, 100),
        };
        // Unders�g Json fil
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save" + activeSave + ".txt", json);

        // Visuelt opdater tekst og knapper
        stressText.text = "Stress: " + saveObject.stressAmount;
        academicText.text = "Academic: " + saveObject.academicAmount;
        socialText.text = "Social: " + saveObject.socialAmount;
        UpdateSaveFiles();
    }
    public void EnterSaveFile(int saveFileId)
    {
        SaveData saveObject;

        if (File.Exists(Application.dataPath + "/Saves/save" + saveFileId + ".txt"))
        { // Load en Save
            // Unders�g Json fil
            string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + saveFileId + ".txt");
            saveObject = JsonUtility.FromJson<SaveData>(saveString);
        }
        else
        { // Lav en ny Save
          // S�t default v�rdier (Skal nok �ndres)
            List<int> historyIndices = new List<int>();
            SD.history.ForEach(scene => historyIndices.Add(this.DH.scenes.IndexOf(scene)));
            saveObject = new SaveData
            {
                flags = FM.flags,
                sentence = SD.DC.SentenceIndex,
                prevScenes = historyIndices,
                stressAmount = 0,
                academicAmount = 0,
                socialAmount = 0
            };

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
    }
    #endregion

    public void EnterChapter(int buttonId)
    {
        DateTime dataCurrent = DateTime.Now; // Få datoen

        if (dataCurrent.Day >= startDate + buttonId)
        {  // Hvis datoen er over startDatoen + ugedag
            Debug.Log("Enter this Chapter");
            // Skift scene
        }
        else
        { // Hvis datoen ikke er over startDatoen + ugedag
            Debug.LogWarning("You can't enter this Chapter");
        }
    }
}
