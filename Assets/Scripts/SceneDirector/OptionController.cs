using SceneDirection;
<<<<<<< HEAD
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

=======
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
>>>>>>> 3692831a8fc5f4036218b97d2e8df0b91b1d61f1
public class OptionController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color defaultColor;
    public Color hoverColor;
    public StoryScene scene;
    public TextMeshProUGUI textMesh;
    public STORYFLAG flag = STORYFLAG.NONE;
    public bool SetFlagTrue;

    public STATCHANGE statChange;
    public int changeAmount;


    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (flag != STORYFLAG.NONE)
            gameObject.GetComponentInParent<OptionSelectionController>().FM.SetFlag(flag, SetFlagTrue);
        ChangeStats();


        gameObject.GetComponentInParent<OptionSelectionController>().PerformOption(scene);

    }
    private void ChangeStats()
    {
<<<<<<< HEAD
        switch (statChange)
        {
            case STATCHANGE.NONE:
                break;
            case STATCHANGE.SOCIAL:
                GameManager.Instance.SocialAmount += changeAmount;
                break;
            case STATCHANGE.ACADEMICS:
                GameManager.Instance.AcademicAmount += changeAmount;
                break;
            case STATCHANGE.STRESS:
                GameManager.Instance.StressAmount += changeAmount;
                break;
        }
=======
        GameManager.Instance.StressAmount += StressChange;
        if (GameManager.Instance.StressAmount < 0)
            GameManager.Instance.StressAmount = 0;
        if (GameManager.Instance.StressAmount > 200)
            GameManager.Instance.StressAmount = 200;

        GameManager.Instance.AcademicAmount += AcademicChange;
        if (GameManager.Instance.AcademicAmount < 0)
            GameManager.Instance.AcademicAmount = 0;
        if (GameManager.Instance.AcademicAmount > 200)
            GameManager.Instance.AcademicAmount = 200;

        GameManager.Instance.SocialAmount += SocialChange;
        if (GameManager.Instance.SocialAmount < 0)
            GameManager.Instance.SocialAmount = 0;
        if (GameManager.Instance.SocialAmount > 200)
            GameManager.Instance.SocialAmount = 200;

>>>>>>> 3692831a8fc5f4036218b97d2e8df0b91b1d61f1
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        textMesh.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMesh.color = defaultColor;
    }
}
