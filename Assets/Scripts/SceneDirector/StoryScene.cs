using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewStoryScene", menuName = "Data/New StoryScene")]
    [System.Serializable]
    public class StoryScene : GameScene
    {
        public List<Sentence> Sentences;
        public Sprite background;
        public GameScene nextScene;

        [System.Serializable]
        public struct Sentence
        {
            public string text;
            public Speaker speaker;

        }
    }

    public class GameScene : ScriptableObject { }
}