using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public void Awake(){
        if (Instance == null || Instance != this)
        {
            Destroy(this);
        }
        else 
            Instance = this;

        Application.targetFrameRate = 60;
    }
}
