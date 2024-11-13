using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="NewStoryScene", menuName ="Data/New StoryScene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    public List<Sentence> Sentences;
    public Sprite background;
    public StoryScene nextScene;

    [System.Serializable]
    public struct Sentence
    {
        public string text;
        public Speaker speaker;

    }
}
