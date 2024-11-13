using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour{

    private class SaveObject {
        public int stressAmount;
        public int academicAmount;
        public int socialAmount;
    }

    [SerializeField] private int startDate;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI academicText;
    [SerializeField] private TextMeshProUGUI socialText;
    [SerializeField] private GameObject[] saveFileButtons;
    [SerializeField] private GameObject[] chapterButtons;

    private int activeSave;

    private void Awake(){
        // Opdater save filer
        UpdateSaveFiles();
        
        // Opdater hvilke chapters man kan spille
        DateTime dataCurrent = DateTime.Now;
        for (int i = 0; i < chapterButtons.Length; i++){
            chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(false);
            if (dataCurrent.Day >= startDate + i){ // Hvis datoen er over startDatoen + ugedage
                chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
            } else { // Hvis datoen ikke er over startDatoen + ugedage
                chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(true);
            }
        }

        if (!Directory.Exists(Application.dataPath + "/Saves/")){ // Opret 'Save' mappe hvis den ikke findes
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        }
    }

    private void UpdateSaveFiles(){
        int maxFiles = 3;
        for (int i = 1; i < maxFiles + 1; i++){
            if (File.Exists(Application.dataPath + "/Saves/save" + i + ".txt")){ // Hvis man har save-filen (i)
                string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + i + ".txt");
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                // Visuelt opdater Save File knapperne
                saveFileButtons[i - 1].GetComponent<SaveFileButtonUI>().UpdateVisual(i, true, saveObject.stressAmount, saveObject.academicAmount, saveObject.socialAmount);
            }
        }
    }

    public void RandomizeValues(){
        SaveObject saveObject = new SaveObject { // Opdaterer værdier
            stressAmount = UnityEngine.Random.Range(1, 100),
            academicAmount = UnityEngine.Random.Range(1, 100),
            socialAmount = UnityEngine.Random.Range(1, 100),
        };
        // Undersøg Json fil
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save" + activeSave + ".txt", json);

        // Visuelt opdater tekst og knapper
        stressText.text = "Stress: " + saveObject.stressAmount;
        academicText.text = "Academic: " + saveObject.academicAmount;
        socialText.text = "Social: " + saveObject.socialAmount;
        UpdateSaveFiles();
    }

    public void EnterSaveFile(int saveFileId){
        SaveObject saveObject;

        if (File.Exists(Application.dataPath + "/Saves/save" + saveFileId + ".txt")){ // Load en Save
            // Undersøg Json fil
            string saveString = File.ReadAllText(Application.dataPath + "/Saves/save" + saveFileId + ".txt");
            saveObject = JsonUtility.FromJson<SaveObject>(saveString);
        } else { // Lav en ny Save
            // Sæt default værdier (Skal nok ændres)
            saveObject = new SaveObject {
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

    public void EnterChapter(int buttonId){
        DateTime dataCurrent = DateTime.Now; // Få datoen

        if (dataCurrent.Day >= startDate + buttonId){  // Hvis datoen er over startDatoen + ugedag
            Debug.Log("Enter this Chapter");
            // Skift scene
        } else { // Hvis datoen ikke er over startDatoen + ugedag
            Debug.LogWarning("You can't enter this Chapter");
        }
    }

    public void Quit(){
        Application.Quit();
    }
}
