using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChooseScene", menuName = "Data/New ChooseScene")]
public class ChooseSceneSO : GameScene {

    public List<ChooseLabel> Options;
    public string QuestionText;
    public AudioClip Music;
    public AudioClip Sound;

    [Serializable]
    public struct ChooseLabel{
        [Header("Main")]
        public string text;
        public StorySceneSO nextScene;

        [Header("Stat Changes")]
        public int StressChange;
        public int AcademicChange;
        public int SocialChange;
    }
}