using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SceneDirection;


[CreateAssetMenu(fileName = "NewDataHolder", menuName = "Data/ New Data Holder")]
[System.Serializable]
public class DataHolder : ScriptableObject
{
    public List<GameScene> scenes;
}

public struct SaveData
{
    public Dictionary<STORYFLAG, Boolean> flags;
    public int sentence;
    public List<int> prevScenes;
    public int stressAmount;
    public int academicAmount;
    public int socialAmount;
    public string characterName;
}