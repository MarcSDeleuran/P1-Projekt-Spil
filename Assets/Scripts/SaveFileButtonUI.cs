using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveFileButtonUI : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI saveFileNameText;
    [SerializeField] private GameObject statsGameObject;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI academicText;
    [SerializeField] private TextMeshProUGUI socialText;

    public void UpdateVisual(int saveFileID, bool activate, int stressAmount, int academicAmount, int socialAmount){
        statsGameObject.SetActive(activate);
        saveFileNameText.text = "Save #" + saveFileID;
        stressText.text = "Stress: " + stressAmount + "%";
        academicText.text = "Academic: " + academicAmount + "%";
        socialText.text = "Social: " + socialAmount + "%";
    }
}
