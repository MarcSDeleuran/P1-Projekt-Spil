using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    public class OptionSelectionController : MonoBehaviour
    {
        public List<GameObject> Options = new List<GameObject>(); 
        public OptionController option;
        public SceneDirector SC;
        private RectTransform rectTransform;
        private Animator animator;
        public TextMeshProUGUI QuestionText;
        public Image CharacterSprite;
        public STORYFLAG[] flags;
        private void Start()
        {
            animator = GetComponent<Animator>();
            rectTransform = GetComponent<RectTransform>();

        }
        public void SetupChoose(ChooseScene scene)
        {
            CharacterSprite.gameObject.SetActive(true);
            DestroyOptions();
            animator.SetTrigger("ShowSelection");
            QuestionText.text = scene.QuestionText;
            QuestionText.gameObject.SetActive(true);
            for (int i = 0; i < scene.Options.Count; i++)
            {
                
                Options[i].GetComponent<OptionController>().scene = scene.Options[i].nextScene;
                Options[i].GetComponent<OptionController>().textMesh.text = scene.Options[i].text;
                Options[i].gameObject.SetActive(true);

            }
        }
        public void PerformOption(StoryScene scene)
        {
            SC.PlayScene(scene);
            animator.SetTrigger("HideSelection");
        }

        private void DestroyOptions()
        {
            foreach (GameObject option in Options)
            {
                option.SetActive(false);
            }
        }
        
    }
    public enum STORYFLAG
    {
        NONE = 0,
        GOODBOY = 1,

    }

}
