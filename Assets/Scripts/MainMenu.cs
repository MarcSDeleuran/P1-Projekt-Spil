using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject blackScreen;
    [Space(5)]
    [SerializeField] private GameObject[] chapterButtons;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI academicText;
    [SerializeField] private TextMeshProUGUI socialText;
    [Space(5)]
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [Space(5)]
    [SerializeField] private AudioMixer audioMixer;

    private bool transitioning = false;
    private int startDate = 25;

    private void Awake(){
        Application.targetFrameRate = 60;

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        masterVolumeSlider.onValueChanged.AddListener( delegate {
            SetMasterVolume(masterVolumeSlider.value);
        });
        sfxVolumeSlider.onValueChanged.AddListener( delegate {
            SetSFXVolume(sfxVolumeSlider.value);
        });
        musicVolumeSlider.onValueChanged.AddListener( delegate {
            SetMusicVolume(musicVolumeSlider.value);
        });

        if (PlayerPrefs.HasKey("MasterVolume")){ // Har spilleren en gemt playerPref (Hvis den ikke har MasterVolume har den ikke nogle af dem)
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume"); // Juster sliderne til den gemte værdi
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        } else { // Hvis spilleren ikke har gemt nogle playerPref (Det er første boot-up)
            PlayerPrefs.SetFloat("MasterVolume", 1f);
            PlayerPrefs.SetFloat("SFXVolume", 1f); // Sæt standard values
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }

        UpdateAvailableChapters();
    }

    private void Start(){
        // Juster volumerne
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
    }

    private void UpdateAvailableChapters(){
        // Opdater hvilke chapters man kan spille
        DateTime dataCurrent = DateTime.Now;
        for (int i = 0; i < chapterButtons.Length; i++){
            chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(false);
            chapterButtons[i].GetComponent<ChapterButtonUI>().dayText.text = "Unlocks: " + (dataCurrent.Day + i - 1) + ". nov";

            // Hvis datoen er over startDatoen + ugedage
            if (dataCurrent.Day >= startDate + i){
                chapterButtons[i].GetComponent<ChapterButtonUI>().unlockedUI.SetActive(true);
            } else { // Hvis ikke
                chapterButtons[i].GetComponent<ChapterButtonUI>().lockedUI.SetActive(true);
            }
        }
    }

    public void StartGame(){
        // Load en Save
        if (PlayerPrefs.HasKey("StressAmount")){
            SavedData.Instance.StressAmount = PlayerPrefs.GetInt("StressAmount");
            SavedData.Instance.AcademicAmount = PlayerPrefs.GetInt("AcademicAmount");
            SavedData.Instance.SocialAmount = PlayerPrefs.GetInt("SocialAmount");
            SavedData.Instance.CharacterName = PlayerPrefs.GetString("CharacterName");
        } else { // Lav en ny Save
            // Sæt default værdier
            int defaultValue = 50;
            PlayerPrefs.SetInt("StressAmount", defaultValue);
            PlayerPrefs.SetInt("AcademicAmount", defaultValue);
            PlayerPrefs.SetInt("SocialAmount", defaultValue);
        }

        // Visuelt opdater tekst
        stressText.text = "Stress: " + PlayerPrefs.GetInt("StressAmount") + "%";
        academicText.text = "Academic: " + PlayerPrefs.GetInt("AcademicAmount") + "%";
        socialText.text = "Social: " + PlayerPrefs.GetInt("SocialAmount") + "%";
    }

    public void RandomizeValues(){
        int newStressAmount = UnityEngine.Random.Range(1, 100);
        int newAcademicAmount = UnityEngine.Random.Range(1, 100);
        int newSocialAmount = UnityEngine.Random.Range(1, 100);

        // Opdaterer værdier
        PlayerPrefs.SetInt("StressAmount", newStressAmount);
        PlayerPrefs.SetInt("AcademicAmount", newAcademicAmount);
        PlayerPrefs.SetInt("SocialAmount", newSocialAmount);

        SavedData.Instance.StressAmount = newStressAmount;
        SavedData.Instance.AcademicAmount = newAcademicAmount;
        SavedData.Instance.SocialAmount = newSocialAmount;

        // Visuelt opdater tekst og knapper
        stressText.text = "Stress: " + newStressAmount + "%";
        academicText.text = "Academic: " + newAcademicAmount + "%";
        socialText.text = "Social: " + newSocialAmount + "%";
    }

    public void EnterChapter(int buttonId){
        DateTime dataCurrent = DateTime.Now; // Få datoen

        // Hvis datoen er over startDatoen + ugedag
        if (dataCurrent.Day >= startDate + buttonId){
            if (!transitioning && chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene != null){
                transitioning = true;
                StartCoroutine(SwitchScene());
                SavedData.Instance.loadedScene = chapterButtons[buttonId].GetComponent<ChapterButtonUI>().ChapterStartScene;
            }
        } else { // Hvis ikke
            Debug.LogWarning("You can't enter this Chapter");
        }
    }

    private IEnumerator SwitchScene(){
        blackScreen.GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("SampleScene");
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
}
