using SceneDirection;
using System.Collections;
using System.Collections.Generic;
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


    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (flag != STORYFLAG.NONE)
            gameObject.GetComponentInParent<OptionSelectionController>().FM.SetFlag(flag, SetFlagTrue);

        gameObject.GetComponentInParent<OptionSelectionController>().PerformOption(scene);

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
