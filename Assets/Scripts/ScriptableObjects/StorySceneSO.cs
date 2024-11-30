using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Data/New StoryScene")]
public class StorySceneSO : GameScene {
    public List<Sentence> Sentences;
    public Sprite background;
    public GameScene nextScene;

    [System.Serializable]
    public struct Sentence {
        [Header("Main")]
        public string text;
        public SpeakerSO speaker;
        public Sprite characterSprite;
        public List<Action> Actions;

        [Header("Stats Change")]
        public int StressChange;
        public int AcademicChange;
        public int SocialChange;

        [Header("Audio")]
        public AudioClip Music;
        public AudioClip Sound;

        [System.Serializable]
        public struct Action{
            public SpeakerSO Speaker;
            public Type ActionType;
            public int SpriteIndex;
            public Vector2 Coords;
            public float MoveSpeed;

            public enum Type {
                NONE, APPEAR, MOVE, FLIP, DISAPPEAR
            }
        }
    }
}

public class GameScene : ScriptableObject { }