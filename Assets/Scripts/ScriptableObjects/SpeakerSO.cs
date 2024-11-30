using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
public class SpeakerSO : ScriptableObject {
    public string speakerName;
    public Color nameColor;
    public Color textColor;
    public List<Sprite> sprites;
    public SpriteController prefab;
    public bool LeftSide = false;
}