using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    public class SpriteSwitcher : MonoBehaviour
    {
        public bool IsSwitched = false;
        public Image Image1;
        public Image Image2;
        private Animator Animator;

        public void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void SwitchImage(Sprite sprite)
        {
            if (!IsSwitched)
            {
                if (Image1.sprite == sprite) return;

                Image2.sprite = sprite;
                Animator.SetTrigger("SwitchFirst");
            }
            else
            {
                if (Image2.sprite == sprite) return;

                Image1.sprite = sprite;
                Animator.SetTrigger("SwitchSecond");
            }
            IsSwitched = !IsSwitched;
        }

        public void SetImage(Sprite sprite)
        {
            if (!IsSwitched)
            {
                Image1.sprite = sprite;
            }
            else
            {
                Image2.sprite = sprite;
            }
        }

        public Sprite GetImage()
        {
            if (!IsSwitched)
            {
                return Image1.sprite;
            }
            else
            {
                return Image2.sprite;
            }
            
        }

        public Sprite GetOtherImage()
        {
            if (IsSwitched)
            {
                return Image1.sprite;
            }
            else
            {
                return Image2.sprite;
            }
        }
    }
}
