using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewFlowScene", menuName = "Data/New FlowScene")]
    [System.Serializable]
    public class FlowScene : GameScene
    {
        public List<SceneReqs> scenes;
        public string QuestionText;
        public AudioClip Music;
        public AudioClip Sound;


        [System.Serializable]
        public struct SceneReqs
        {
            public StoryScene nextScene;

            [Header("Stat Reqs")]
            public int StressChange;
            public int AcademicChange;
            public int SocialChange;

            [Header("Flag Reqs")]
            public STORYFLAG flag;
            public bool setFlagTrue;
            public STORYFLAG FlagToSet;
            public Sprite ChoiceIcon;
        }
    }
}





