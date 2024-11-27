using UnityEngine;
using UnityEngine.UIElements;

namespace SceneDirection
{
    [CreateAssetMenu(fileName = "NewWriteScene", menuName = "Data/New WriteScene")]
    [System.Serializable]
    public class WriteScene : GameScene
    {
        public string QuestionText;
        public AudioClip Music;
        public AudioClip Sound;
        public GameScene NextScene;
        public TextField nameField;

    }
}
