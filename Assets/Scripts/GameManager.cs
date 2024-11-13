using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    [SerializeField] private int startDate;
    [SerializeField] private GameObject[] chapterButtons;

    private void Awake(){
        DateTime dataCurrent = DateTime.Now;

        for (int i = 0; i < chapterButtons.Length; i++){
            chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(false);
            if (dataCurrent.Day >= startDate + i){
                chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
            } else {
                chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(true);
            }
        }

        if (!Directory.Exists(Application.dataPath + "/Saves/")){
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        }
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.S)){
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L)){
            Load();
        }
    }

    private void Save(){
        int stressAmount = UnityEngine.Random.Range(1, 100);

        SaveObject saveObject = new SaveObject {
            stressAmount = stressAmount
        };
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/save.txt", json);

        Debug.Log("SAVED");
    }

    private void Load(){
        string saveString = null;

        if (File.Exists(Application.dataPath + "/Saves/save.txt")){
            saveString = File.ReadAllText(Application.dataPath + "/Saves/save.txt");
        }

        if (saveString != null){
            Debug.Log("LOADED");

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            Debug.Log("Stress amount: " + saveObject.stressAmount);
        } else {
            Debug.LogWarning("No save found");
        }
    }

    public void EnterChapter(int dateToUnlock){
        DateTime dataCurrent = DateTime.Now;

        if (dataCurrent.Day >= dateToUnlock){
            Debug.Log("Enter this Chapter");
        } else {
            Debug.LogWarning("You can't enter this Chapter");
        }
    }

    private class SaveObject {
        public int stressAmount;
    }
}
