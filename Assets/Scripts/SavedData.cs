using System.Collections.Generic;
using UnityEngine;

public class SavedData : MonoBehaviour {

    public static SavedData Instance { get; private set; }

    [Header("Data")]
    public string CharacterName;
    [Space(5)]
    public int StressAmount = 50;
    public int AcademicAmount = 50;
    public int SocialAmount = 50;
    [Space(5)]
    public StorySceneSO loadedScene;

    private void Awake(){
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
