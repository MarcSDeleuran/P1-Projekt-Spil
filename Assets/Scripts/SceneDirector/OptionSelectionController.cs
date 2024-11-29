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
        public FlagManager FM;
        private RectTransform rectTransform;
        private Animator animator;
        public TextMeshProUGUI QuestionText;
        public Image CharacterSprite;
        public GameObject Choice;
        public GameObject ChoiceContainer;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rectTransform = GetComponent<RectTransform>();

        }
        //public void SetupChoose(ChooseScene scene)
        //{
        //    CharacterSprite.gameObject.SetActive(true);
        //    DestroyOptions();
        //    animator.SetTrigger("ShowSelection");
        //    QuestionText.text = scene.QuestionText;
        //    QuestionText.gameObject.SetActive(true);
        //    for (int i = 0; i < scene.Options.Count; i++)
        //    {

        //        if (scene.Options[i].flag == STORYFLAG.NONE || FM.CheckFlag(scene.Options[i].flag))
        //        {
        //            Options[i].GetComponent<OptionController>().scene = scene.Options[i].nextScene;
        //            Options[i].GetComponent<OptionController>().textMesh.text = scene.Options[i].text;
        //            Options[i].gameObject.SetActive(true);
        //            if (scene.Options[i].FlagToSet != STORYFLAG.NONE)
        //            {
        //                Options[i].GetComponent<OptionController>().flag = scene.Options[i].FlagToSet;
        //                Options[i].GetComponent<OptionController>().SetFlagTrue = scene.Options[i].setFlagTrue;
        //            }

        //        }

        //        Options[i].GetComponent<OptionController>().StressChange = scene.Options[i].StressChange;
        //        Options[i].GetComponent<OptionController>().AcademicChange = scene.Options[i].AcademicChange;
        //        Options[i].GetComponent<OptionController>().SocialChange = scene.Options[i].SocialChange;


        //    }
        //}
        public void SetupChoose(ChooseScene scene)
        {

            DestroyOptions();


            CharacterSprite.gameObject.SetActive(true);
            QuestionText.text = scene.QuestionText;
            QuestionText.gameObject.SetActive(true);


            foreach (var option in scene.Options)
            {
                if (option.flag == STORYFLAG.NONE || FM.CheckFlag(option.flag))
                {

                    GameObject newButton = Instantiate(Choice, ChoiceContainer.transform); 
                    OptionController optionController = newButton.GetComponent<OptionController>();

                    optionController.scene = option.nextScene;
                    optionController.flag = option.FlagToSet;
                    optionController.SetFlagTrue = option.setFlagTrue;
                    optionController.StressChange = option.StressChange;
                    optionController.AcademicChange = option.AcademicChange;
                    optionController.SocialChange = option.SocialChange;

 
                    TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                    buttonText.text = option.text;


                    newButton.gameObject.SetActive(true);
                }
            }


            animator.SetTrigger("ShowSelection");
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


}
