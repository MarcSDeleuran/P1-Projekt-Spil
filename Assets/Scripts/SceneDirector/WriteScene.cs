using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewWriteScene", menuName = "Data/New ChooseScene")]
    [System.Serializable]
    public class WriteScene : GameScene
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
        }
    }

}
