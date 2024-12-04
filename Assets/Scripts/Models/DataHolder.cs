using System;
using System.Collections.Generic;
using UnityEngine;
using SceneDirection;

[CreateAssetMenu(fileName = "NewDataHolder", menuName = "Data/ New Data Holder")]
[Serializable]
public class DataHolder : ScriptableObject
{
    public List<GameScene> scenes;
}

public struct SaveData
{
    public int saveFileId;
    public List<StoryFlag> flags;
    public int sentence;
    public List<int> prevScenes;
    public int stressAmount;
    public int academicAmount;
    public int socialAmount;
    public string characterName;
    public bool[] chapterCompletes;
}
[Serializable]
public struct StoryFlag
{
    public string key;
    public bool value;
}