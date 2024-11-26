using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerHolder : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
