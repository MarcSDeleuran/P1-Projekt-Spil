using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField] private Button quitButton;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [Space(5)]
    [SerializeField] private AudioMixer audioMixer;

    private void Awake(){
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
    }

    private void Start(){
        // Juster volumerne
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
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
