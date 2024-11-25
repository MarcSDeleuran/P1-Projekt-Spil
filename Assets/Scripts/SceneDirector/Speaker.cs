using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
    [System.Serializable]

    public class Speaker : ScriptableObject
    {
        public string speakerName;
        public Color textColor;
        public List<Sprite> sprites;
        public SpriteController prefab;
        public bool LeftSide = false;
    }
}
