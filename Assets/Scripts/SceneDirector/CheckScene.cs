using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewCheckScene", menuName = "Data/New CheckScene")]
    [System.Serializable]
    public class CheckScene : GameScene
    {
        public List<Reqs> Requirement;
        public string QuestionText;
        public AudioClip Music;
        public AudioClip Sound;
        public StoryScene DefaultScene;

        [System.Serializable]
        public struct Reqs
        {
            public string text;
            public StoryScene nextScene;
            public STORYFLAG flag;



            public int StressReq;
            public int AcademicReq;
            public int SocialReq;
            public int StressUnderReq;
            public int AcademicUnderReq;
            public int SocialUnderReq;
        }
    }
}





