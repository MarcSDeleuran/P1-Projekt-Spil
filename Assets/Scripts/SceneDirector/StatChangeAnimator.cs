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

        public void ShowStatChange(string statName, bool isIncrease)
        {
            bool inverseColors = statName == "statName";

            GameObject statChangeInstance = Instantiate(statChangePrefab, statChangeContainer);
            StatChangeUI statChangeComponent = statChangeInstance.GetComponent<StatChangeUI>();
            statChangeComponent.statText.text = statName;
            statChangeComponent.goodArrow.SetActive(isIncrease);
            statChangeComponent.badArrow.SetActive(!isIncrease);

            if (inverseColors){
                statChangeComponent.goodArrow.GetComponent<Image>().color = statChangeComponent.badColor;
                statChangeComponent.badArrow.GetComponent<Image>().color = statChangeComponent.goodColor;
            }
        }
    }
}
