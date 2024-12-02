using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatChangeUI : MonoBehaviour {

    public TextMeshProUGUI statText;
    public GameObject goodArrow;
    public GameObject badArrow;
    public Color goodColor;
    public Color badColor;

    private void Start(){
        Destroy(gameObject, 4);
    }
}
