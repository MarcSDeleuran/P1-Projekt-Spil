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
        public Sprite currentImage;

        public void Awake()
        {
            Animator = GetComponent<Animator>();
            
        }

        public void SwitchImage(Sprite sprite)
        {
            if (sprite == GetImage())
                return;
            if (!IsSwitched)
            {
                Image2.sprite = sprite;
                Animator.SetTrigger("SwitchFirst");
            }
            else
            {
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
                Debug.Log(Image1.sprite.name);
                return Image1.sprite;
            }
            else
            {
                Debug.Log(Image2.sprite.name);
                return Image2.sprite;
            }
            
        }

    }
}
