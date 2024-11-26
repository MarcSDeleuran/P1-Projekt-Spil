using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    [SerializeField] private Button statsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject statsUI;
    [Space(5)]
    [SerializeField] private Slider stressSlider;
    [SerializeField] private Slider academicSlider;
    [SerializeField] private Slider socialSlider;
    private bool inStats;

    private void Awake(){
        statsButton.onClick.AddListener(() => {
            StartCoroutine(WaitBeforeUpdating());
            statsUI.SetActive(true);

            // Start fra bunden
            stressSlider.value = 0;
            academicSlider.value = 0;
            socialSlider.value = 0;
        });
        backButton.onClick.AddListener(() => {
            inStats = false;
            statsUI.SetActive(false);
        });
    }

    private IEnumerator WaitBeforeUpdating(){
        yield return new WaitForSeconds(0.25f);

        inStats = true;
    }

    private void Update(){
        if (inStats){
            float stressScore = 50f; // Skal udskiftes med score score
            if (stressSlider.value != stressScore){
                stressSlider.value = Mathf.Lerp(stressSlider.value, stressScore, 4f * Time.deltaTime);
            }
            float academicScore = 50f; // Skal udskiftes med academic score
            if (academicSlider.value != academicScore){
                academicSlider.value = Mathf.Lerp(academicSlider.value, academicScore, 4f * Time.deltaTime);
            }
            float socialScore = 50f; // Skal udskiftes med social score
            if (socialSlider.value != socialScore){
                socialSlider.value = Mathf.Lerp(socialSlider.value, socialScore, 4f * Time.deltaTime);
            }
        }
    }
}
