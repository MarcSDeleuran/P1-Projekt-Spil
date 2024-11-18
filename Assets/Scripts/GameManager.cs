using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance;

    public static GameManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Destroy(this);
        }
        else 
            Instance = this;

        Application.targetFrameRate = 60;
    }


}
