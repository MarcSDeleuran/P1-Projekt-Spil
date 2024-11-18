﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewChooseScene", menuName = "Data/New ChooseScene")]
    [System.Serializable]
    public class ChooseScene : GameScene
    {
        public List<ChooseLabel> Options;
        public string QuestionText;


        [System.Serializable]
        public struct ChooseLabel
        {
            public string text;
            public StoryScene nextScene;
         
        }
    }
}