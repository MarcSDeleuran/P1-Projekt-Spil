using SceneDirection;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public StoryScene scene;
    public TextMeshProUGUI textMesh;
    public STORYFLAG flag = STORYFLAG.NONE;
    public bool SetFlagTrue;

    public int StressChange;
    public int AcademicChange;
    public int SocialChange;
    public Image ChoiceIcon;

    private void Awake(){
        GetComponent<Button>().onClick.AddListener(() => {
            if (flag != STORYFLAG.NONE){
                gameObject.GetComponentInParent<OptionSelectionController>().FM.SetFlag(flag, SetFlagTrue);
            }
            
            ChangeStats();
            gameObject.GetComponentInParent<OptionSelectionController>().PerformOption(scene);
        });
    }

    private void ChangeStats()
    {
        StatChangeAnimator STA = GameManager.Instance.STA;

        GameManager.Instance.StressAmount += StressChange;
        if (GameManager.Instance.StressAmount < 0)
            GameManager.Instance.StressAmount = 0;
        if (GameManager.Instance.StressAmount > 200)
            GameManager.Instance.StressAmount = 200;

        if (StressChange > 0 && StressChange != 0)
            STA.ShowStatChange("Stress", true);
        else if (StressChange != 0)
            STA.ShowStatChange("Stress", false);

        GameManager.Instance.AcademicAmount += AcademicChange;
        if (GameManager.Instance.AcademicAmount < 0)
            GameManager.Instance.AcademicAmount = 0;
        if (GameManager.Instance.AcademicAmount > 200)
            GameManager.Instance.AcademicAmount = 200;

        if (AcademicChange > 0 && AcademicChange != 0)
            STA.ShowStatChange("Academics", true);
        else if (StressChange != 0)
            STA.ShowStatChange("Academics", false);

        GameManager.Instance.SocialAmount += SocialChange;
        if (GameManager.Instance.SocialAmount < 0)
            GameManager.Instance.SocialAmount = 0;
        if (GameManager.Instance.SocialAmount > 200)
            GameManager.Instance.SocialAmount = 200;


        if (SocialChange > 0 && SocialChange != 0)
            STA.ShowStatChange("Social", true);
        else if (StressChange != 0)
            STA.ShowStatChange("Social", false);
    }
}
