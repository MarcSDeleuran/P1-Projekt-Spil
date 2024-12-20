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
        public bool OpenJournal = false;
        public bool FinalScene = false;
        public int FinalSceneChapterId;

        [System.Serializable]
        public struct Sentence
        {
            public string text;
            public Speaker speaker;
            public List<Action> Actions;

            public AudioClip Music;
            public AudioClip Sound;

            public int StressChange;
            public int AcademicChange;
            public int SocialChange;

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
                    NONE, APPEAR, MOVE, FLIP, DISAPPEAR
                }
            }
        }
    }

    public class GameScene : ScriptableObject { }
}