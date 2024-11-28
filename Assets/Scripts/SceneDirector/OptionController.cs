using SceneDirection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
