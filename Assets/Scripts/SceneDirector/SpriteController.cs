using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity;


namespace SceneDirection
{
    public class SpriteController : MonoBehaviour
    {
        private SpriteSwitcher switcher;
        private RectTransform rect;
        private Animator animator;
        public bool hidden;

        public void Awake()
        {
            switcher = GetComponent<SpriteSwitcher>();
            animator = GetComponent<Animator>();
            rect = GetComponent<RectTransform>();
            animator.SetFloat("speedmultiplier", GameManager.Instance.animationMultiplier);
        }

        public void Setup(Sprite sprite)
        {
            switcher.SetImage(sprite);
        }

        public void Show(Vector2 coords)
        {
            animator.SetTrigger("ShowCharacter");
            hidden = false;
            rect.localPosition = coords;
        }

        public void Hide()
        {
            animator.SetTrigger("HideCharacter");
            hidden = true;
        }

        public void Move(Vector2 coords, float speed)
        {
            StartCoroutine(MoveCoroutine(coords, speed));
        }

        private IEnumerator MoveCoroutine(Vector2 coords, float speed)
        {
            while (rect.localPosition.x != coords.x || rect.localPosition.y != coords.y)
            {
                rect.localPosition = Vector2.MoveTowards(rect.localPosition, coords, Time.deltaTime * 1000f * speed);
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void SwitchSprite(Sprite sprite)
        {
            if (switcher.GetImage() != sprite)
            {
                switcher.SwitchImage(sprite);
            }
        }
    }
}

