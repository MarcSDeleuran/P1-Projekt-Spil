using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour {

    [Header("Stats")]
    public int StressChange;
    public int AcademicChange;
    public int SocialChange;

    [Header("References")]
    public StorySceneSO scene;
    public TextMeshProUGUI textMesh;

    private void Awake(){
        GetComponent<Button>().onClick.AddListener(() => {
            ChangeStats();
            GameManager.Instance.PlayGameScene(scene);
            GameManager.Instance.choiceUI.GetComponent<Animator>().SetTrigger("Out");
        });
    }

    private void ChangeStats(){
        SavedData.Instance.StressAmount += StressChange;
        if (SavedData.Instance.StressAmount < 0){
            SavedData.Instance.StressAmount = 0;
        }
            
        if (SavedData.Instance.StressAmount > 200){
            SavedData.Instance.StressAmount = 200;
        }

        if (StressChange > 0 && StressChange != 0){
            GameManager.Instance.ShowStatChange("Stress", true, true);
        } else if (StressChange != 0){
            GameManager.Instance.ShowStatChange("Stress", false, true);
        }
            
        SavedData.Instance.AcademicAmount += AcademicChange;
        if (SavedData.Instance.AcademicAmount < 0){
            SavedData.Instance.AcademicAmount = 0;
        }

        if (SavedData.Instance.AcademicAmount > 200){
            SavedData.Instance.AcademicAmount = 200;
        }

        if (AcademicChange > 0 && AcademicChange != 0){
            GameManager.Instance.ShowStatChange("Academics", true, false);
        } else if (StressChange != 0){
            GameManager.Instance.ShowStatChange("Academics", false, false);
        }
            
        SavedData.Instance.SocialAmount += SocialChange;
        if (SavedData.Instance.SocialAmount < 0){
            SavedData.Instance.SocialAmount = 0;
        }
            
        if (SavedData.Instance.SocialAmount > 200){
            SavedData.Instance.SocialAmount = 200;
        } 

        if (SocialChange > 0 && SocialChange != 0){
            GameManager.Instance.ShowStatChange("Social", true, false);
        } else if (StressChange != 0){
            GameManager.Instance.ShowStatChange("Social", false, false);
        }
    }
}
