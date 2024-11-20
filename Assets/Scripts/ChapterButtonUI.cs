using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChapterButtonUI : MonoBehaviour{

    [Header("References")]
    [SerializeField] private int buttonID;
    public GameObject unlockedUI;
    public GameObject lockedUI;
    [SerializeField] private TextMeshProUGUI unlockText;

    private void Start(){
        unlockText.text = "Unlocks: " + (MainMenu.Instance.GetStartDate() + buttonID) + ". nov";
    }

    public void EnterChapter(int button){
        DateTime dataCurrent = DateTime.Now; // Få datoen
        if (dataCurrent.Day >= MainMenu.Instance.GetStartDate() + button){  // Hvis datoen er over startDatoen + ugedag
            SceneManager.LoadScene("SampleScene");
        } else { // Hvis datoen ikke er over startDatoen + ugedag
            Debug.LogWarning("You can't enter this Chapter");
        }
    }
}
