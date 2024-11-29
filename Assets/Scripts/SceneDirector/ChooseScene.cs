using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
            [Header("Main")]
            public string text;
            public StoryScene nextScene;

            [Header("Stat Changes")]
            public int StressChange;
            public int AcademicChange;
            public int SocialChange;

            [Header("Flags")]
            public STORYFLAG flag;
            public bool setFlagTrue;
            public STORYFLAG FlagToSet;
            public Sprite ChoiceIcon;
        }
    }
}





