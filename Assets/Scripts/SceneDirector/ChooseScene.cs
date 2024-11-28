using System.Collections.Generic;
using UnityEngine;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewChooseScene", menuName = "Data/New ChooseScene")]
    [System.Serializable]
    public class ChooseScene : GameScene
    {
        public List<ChooseLabel> Options;
        public string QuestionText;
        public AudioClip Music;
        public AudioClip Sound;

        [System.Serializable]
        public struct ChooseLabel
        {
            public string text;
            public StoryScene nextScene;
            public STORYFLAG flag;

            public bool setFlagTrue;
            public STORYFLAG FlagToSet;

            public STATCHANGE statChange;
            public int changeAmount;
        }
    }

    public enum STATCHANGE
    {
        NONE,
        SOCIAL,
        ACADEMICS,
        STRESS
    }
}
