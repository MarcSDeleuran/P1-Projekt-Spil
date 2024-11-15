using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    public class SpriteController : MonoBehaviour
    {
        private RectTransform rect;
        public void Awake()
        {
            rect = GetComponent<RectTransform>();
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
        public void SetPosition(Vector2 pos)
        {
            rect.localPosition = pos;
        }
        public void SwitchSprite(Sprite sprite)
        {
            if (GetComponentInChildren<Image>().sprite != sprite)
            {
                GetComponentInChildren<Image>().sprite = sprite;
            }
        }
    }
}

