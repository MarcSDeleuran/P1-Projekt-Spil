using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public bool IsSwitched = false;
    public Image Background1;
    public Image Background2;
    public Animator Animator;

    public void SwitchImage(Sprite sprite)
    {
        if (!IsSwitched)
        {
            Background2.sprite = sprite;
            Animator.SetTrigger("SwitchFirst");
        }
        else
        {
            Background1.sprite = sprite;
            Animator.SetTrigger("SwitchSecond");
        }
        IsSwitched = !IsSwitched;
    }
    public void SetImage(Sprite sprite)
    {
        if (!IsSwitched)
        {
            Background1.sprite = sprite;

        }
        else
        {
            Background2.sprite = sprite;

        }
    }
}
