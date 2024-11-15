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
            public List<Action> Actions;

            [System.Serializable]
            public struct Action
            {
                public Speaker Speaker;
                public Type ActionType;
                public int SpriteIndex;
                public Vector2 Coords;
                public float MoveSpeed;

                [System.Serializable]
                public enum Type
                {
                    NONE, APPEAR, MOVE, DISAPPEAR
                }
            }

        }
    }

    public class GameScene : ScriptableObject { }
}