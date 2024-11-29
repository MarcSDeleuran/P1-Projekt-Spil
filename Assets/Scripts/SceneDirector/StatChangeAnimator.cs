using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneDirection
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class StatChangeAnimator : MonoBehaviour
    {
        public GameObject statChangePrefab; // Assign the prefab here
        public Transform statChangeContainer; // Assign the container in the Canvas
        public float animationDuration = 1.5f; // Duration of the animation
        public float verticalSpacing = 30f; // Spacing between rows

        private Queue<GameObject> activeAnimations = new Queue<GameObject>();


        public void ShowStatChange(string statName, bool isIncrease)
        {

            GameObject statChangeInstance = Instantiate(statChangePrefab, statChangeContainer);
            TMP_Text textComponent = statChangeInstance.GetComponentInChildren<TMP_Text>();
            Image arrowImage = statChangeInstance.GetComponentInChildren<Image>();


            textComponent.text = statName;
            arrowImage.transform.localRotation = isIncrease ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
            arrowImage.color = isIncrease ? Color.green : Color.red;


            StartCoroutine(AnimateStatChange(statChangeInstance));
        }

        private IEnumerator AnimateStatChange(GameObject statChangeInstance)
        {

            activeAnimations.Enqueue(statChangeInstance);


            UpdatePositions();


            CanvasGroup canvasGroup = statChangeInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = statChangeInstance.AddComponent<CanvasGroup>();
            }


            float elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / (animationDuration / 2));
                yield return null;
            }


            yield return new WaitForSeconds(animationDuration / 2);


            elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / (animationDuration / 2));
                yield return null;
            }


            Destroy(statChangeInstance);
            activeAnimations.Dequeue();
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            int index = 0;
            foreach (GameObject anim in activeAnimations)
            {
                RectTransform rectTransform = anim.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -index * verticalSpacing);
                index++;
            }
        }
    }
}
