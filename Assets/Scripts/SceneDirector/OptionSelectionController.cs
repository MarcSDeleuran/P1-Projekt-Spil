using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SceneDirection
{
    public class OptionSelectionController : MonoBehaviour
    {
        public OptionController[] option;
        public SceneDirector SC;
        public FlagManager FM;
        private Animator animator;
        public TextMeshProUGUI QuestionText;
        public Image CharacterSprite;
        public GameObject ChoiceContainer;
        public RectTransform choicesBackground;

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger("Hide");
        }

        public void SetupChoose(ChooseScene scene)
        {
            CharacterSprite.gameObject.SetActive(true);
            QuestionText.text = scene.QuestionText;
            QuestionText.gameObject.SetActive(true);

            float choicesBackgroundHeight = 20 + (60 * scene.Options.Count);
            choicesBackground.sizeDelta = new Vector3(1720, choicesBackgroundHeight);
            QuestionText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, choicesBackgroundHeight + 490);

            foreach (Transform child in ChoiceContainer.transform){
                GameObject referencedButton = child.gameObject;
                OptionController optionController = referencedButton.GetComponent<OptionController>();
                optionController.textMesh.text = "";

                optionController.scene = null;
                optionController.SetFlagTrue = false;
                optionController.StressChange = 0;
                optionController.AcademicChange = 0;
                optionController.SocialChange = 0;
                referencedButton.SetActive(false);
            }

            for (int i = 0; i < scene.Options.Count; i++){
                GameObject referencedButton = ChoiceContainer.transform.GetChild(i).gameObject;
                OptionController optionController = referencedButton.GetComponent<OptionController>();
                optionController.textMesh.text = scene.Options[i].text;

                referencedButton.SetActive(true);
                optionController.scene = scene.Options[i].nextScene;
                optionController.flag = scene.Options[i].FlagToSet;
                optionController.SetFlagTrue = scene.Options[i].setFlagTrue;
                optionController.StressChange = scene.Options[i].StressChange;
                optionController.AcademicChange = scene.Options[i].AcademicChange;
                optionController.SocialChange = scene.Options[i].SocialChange;

                Debug.Log(scene.Options[i]);
                Debug.Log(scene.Options[i].ChoiceIcon);
                if (scene.Options[i].ChoiceIcon != null){
                    optionController.ChoiceIcon.sprite = scene.Options[i].ChoiceIcon;
                } else {
                    optionController.ChoiceIcon.gameObject.SetActive(false);
                }
 
                optionController.textMesh.text = scene.Options[i].text;
                if (option[i].flag != STORYFLAG.NONE /*|| FM.CheckFlag(option[i].flag)*/){
                    optionController.textMesh.color = Color.red;
                }
            }

            animator.SetTrigger("ShowSelection");
        }
        
        public void PerformOption(StoryScene scene)
        {
            SC.PlayScene(scene);
            animator.SetTrigger("HideSelection");
        }
    }
}
