using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private class SaveObject
    {
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

    

    private void Awake()
    {
        // Opdater save filer

    }

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







    public void Quit()
    {
        Application.Quit();
    }
}
