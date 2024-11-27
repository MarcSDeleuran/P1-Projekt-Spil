using UnityEngine;

public class ManagerHolder : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
