using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [Header("Current scene")]
    [SerializeField] private SceneState state = SceneState.Idle;
    [Space(5)]
    public GameScene currentScene;
    [SerializeField] private StorySceneSO storyCurrentScene;

    [Header("References")]
    [SerializeField] private SpriteSwitcher spriteSwitcher;
    [SerializeField] private AudioManager AudioManager;
    [Space(5)]
    public GameObject choiceUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject ChoiceContainer;
    [SerializeField] private GameObject leftSpeakerName;
    [SerializeField] private GameObject rightSpeakerName;
    [SerializeField] private GameObject NameField;
    [SerializeField] private GameObject pauseUI;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private TextMeshProUGUI QuestionText;
    [SerializeField] private TextMeshProUGUI LeftSpeakerNameText;
    [SerializeField] private TextMeshProUGUI RightSpeakerNameText;
    [Space(5)]
    [SerializeField] private Image leftSpeakerNameBackground;
    [SerializeField] private Image rightSpeakerNameBackground;
    [SerializeField] private Image CharacterSprite;
    [Space(5)]
    [SerializeField] private GameObject statChangePrefab;
    [SerializeField] private Transform statChangeContainer;
    [SerializeField] private RectTransform choicesBackground;

    private Coroutine typingCoroutine;
    private int sentenceIndex = -1;

    [SerializeField] private enum SceneState { Idle, Typing, Transitioning }

    public int SentenceIndex { get { return sentenceIndex; } }

    private bool isPaused = false;
    private bool isCharacterFlipped = false;

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        if (SavedData.Instance == null){
            SceneManager.LoadScene("MainMenu");
            return;
        }

        StartCoroutine(PlayStoryScene(SavedData.Instance.loadedScene));
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            if (state == SceneState.Idle){
                bool isLastSentence = sentenceIndex + 1 == storyCurrentScene.Sentences.Count;
                if (isLastSentence){
                    PlayGameScene((currentScene as StorySceneSO).nextScene);
                } else {
                    string text = storyCurrentScene.Sentences[++sentenceIndex].text.Replace("[PlayerName]", SavedData.Instance.CharacterName);
                    PlayNextSentence(text);
                    PlayAudio((currentScene as StorySceneSO).Sentences[SentenceIndex]);
                }
            } else if (state == SceneState.Typing){ // Fast forward
                StopCoroutine(typingCoroutine);
                DialogueText.text = storyCurrentScene.Sentences[sentenceIndex].text.Replace("[PlayerName]", SavedData.Instance.CharacterName);
                state = SceneState.Idle;
            }
        } else if (Input.GetKeyDown(KeyCode.Escape)){
            isPaused = !isPaused;
            pauseUI.SetActive(isPaused);
        }
    }

    public IEnumerator PlayStoryScene(StorySceneSO scene){
        currentScene = scene;
        storyCurrentScene = scene;
        sentenceIndex = -1;

        dialogueUI.SetActive(true);
        dialogueUI.GetComponent<Animator>().SetTrigger("Show");

        string text = storyCurrentScene.Sentences[++sentenceIndex].text.Replace("[PlayerName]", SavedData.Instance.CharacterName);
        SpeakerSO speaker = storyCurrentScene.Sentences[sentenceIndex].speaker;

        yield return new WaitForSeconds(1f);

        HandleSpeakerAvatar(speaker);

        PlayNextSentence(text);
    }

    private void HandleSpeakerAvatar(SpeakerSO speaker){
        if (speaker != null){
            if (speaker.LeftSide){
                LeftSpeakerNameText.text = speaker.speakerName;
                leftSpeakerNameBackground.color = speaker.nameColor;
            } else if (!speaker.LeftSide){
                RightSpeakerNameText.text = speaker.speakerName;
                rightSpeakerNameBackground.color = speaker.nameColor;
            }

            DialogueText.color = speaker.textColor;
            leftSpeakerName.SetActive(speaker.LeftSide);
            rightSpeakerName.SetActive(!speaker.LeftSide);

            if (speaker.speakerName != "You"){ // Man skal ikke se main karakterens sprite
                CharacterSprite.sprite = speaker.sprites[Random.Range(0, speaker.sprites.Count)]; // Det her skal reworkes en smule, det burde ikke være random hvilket sprite af characteren der vises
            }
        } else {
            leftSpeakerName.SetActive(false);
            rightSpeakerName.SetActive(false);
            DialogueText.color = Color.white;
        }
    }

    private void PlayNextSentence(string message){
        typingCoroutine = StartCoroutine(TypeText(message));
        SpeakerSO speaker = storyCurrentScene.Sentences[sentenceIndex].speaker;
        HandleSpeakerAvatar(speaker);

        List<StorySceneSO.Sentence.Action> actions = storyCurrentScene.Sentences[sentenceIndex].Actions;
        foreach (var action in actions){
            HandleSpeakerAction(action);
        }

        ChangeStats(storyCurrentScene.Sentences[sentenceIndex].SocialChange, storyCurrentScene.Sentences[sentenceIndex].AcademicChange, storyCurrentScene.Sentences[sentenceIndex].StressChange);
    }

    private void ChangeStats(int social, int academic, int stress){
        UpdateStat(ref SavedData.Instance.StressAmount, stress, "Stress");
        UpdateStat(ref SavedData.Instance.AcademicAmount, academic, "Academics");
        UpdateStat(ref SavedData.Instance.SocialAmount, social, "Social");
    }

    private void UpdateStat(ref int stat, int change, string statName){
        bool isStress = statName == "Stress";

        stat += change;
        stat = Mathf.Clamp(stat, 0, 200);
        if (change != 0){
            ShowStatChange(statName, change > 0, isStress);
        }
    }

    public void ShowStatChange(string statName, bool isIncrease, bool inverseColors){
        GameObject statChange = Instantiate(statChangePrefab, statChangeContainer);
        StatChangeUI statChangeComponent = statChange.GetComponent<StatChangeUI>();
        statChangeComponent.statText.text = statName;
        statChangeComponent.goodArrow.SetActive(isIncrease);
        statChangeComponent.badArrow.SetActive(!isIncrease);

        if (inverseColors){ // Dette er pga. at højere stress er værre
            statChangeComponent.goodArrow.GetComponent<Image>().color = statChangeComponent.badColor;
            statChangeComponent.badArrow.GetComponent<Image>().color = statChangeComponent.goodColor;
        }
    }

    public void PlayGameScene(GameScene scene){
        StartCoroutine(SwitchScene(scene));
    }

    private IEnumerator SwitchScene(GameScene scene){
        state = SceneState.Transitioning;
        currentScene = scene;

        switch (scene){
            case WriteSceneSO:
                DialogueText.text = "";
                dialogueUI.GetComponent<Animator>().SetTrigger("Hide");
                leftSpeakerName.SetActive(false);
                rightSpeakerName.SetActive(false);

                yield return new WaitForSeconds(1f);

                dialogueUI.SetActive(false);
                NameField.SetActive(true);
                break;
            case StorySceneSO:
                StorySceneSO storyScene = (StorySceneSO)scene;

                DialogueText.text = "";
                dialogueUI.GetComponent<Animator>().SetTrigger("Hide");
                leftSpeakerName.SetActive(false);
                rightSpeakerName.SetActive(false);

                if (spriteSwitcher.GetImage() != storyScene.background){
                    spriteSwitcher.SwitchImage(storyScene.background);
                }

                yield return new WaitForSeconds(1f);

                dialogueUI.SetActive(false);
                PlayAudio(storyScene.Sentences[0]);

                StartCoroutine(PlayStoryScene(storyScene));

                break;
            case ChooseSceneSO:
                PlayAudio(scene as ChooseSceneSO);
                SetupChoose(scene as ChooseSceneSO);
                break;
        }
    }

    private void SetupChoose(ChooseSceneSO scene){
        NameField.SetActive(false);
        dialogueUI.SetActive(false);
        choiceUI.SetActive(false); // Nulstiller animatoren
        choiceUI.SetActive(true);
        QuestionText.text = scene.QuestionText;
        QuestionText.gameObject.SetActive(true);

        float choicesBackgroundHeight = 20 + (60 * scene.Options.Count);
        choicesBackground.sizeDelta = new Vector2(1720, choicesBackgroundHeight);
        QuestionText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, choicesBackgroundHeight + 490);

        for (int i = 0; i < scene.Options.Count; i++){
            GameObject referencedButton = ChoiceContainer.transform.GetChild(i).gameObject;
            OptionController optionController = referencedButton.GetComponent<OptionController>();
            optionController.textMesh.text = scene.Options[i].text;

            referencedButton.SetActive(true);
            optionController.scene = scene.Options[i].nextScene;
            optionController.StressChange = scene.Options[i].StressChange;
            optionController.AcademicChange = scene.Options[i].AcademicChange;
            optionController.SocialChange = scene.Options[i].SocialChange;
        }
    }

    private IEnumerator TypeText(string text){
        DialogueText.text = "";
        state = SceneState.Typing;

        foreach (char c in text){
            DialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        state = SceneState.Idle;
    }

    private void SaveName(){
        if (NameField.GetComponentInChildren<TMP_InputField>().text != ""){
            PlayerPrefs.SetString("CharacterName", NameField.GetComponentInChildren<TMP_InputField>().text);
            SavedData.Instance.CharacterName = NameField.GetComponentInChildren<TMP_InputField>().text;
            WriteSceneSO ws = currentScene as WriteSceneSO;
            PlayGameScene(ws.NextScene);
            NameField.GetComponent<Animator>().SetTrigger("Out");
        }
    }

    private void HandleSpeakerAction(StorySceneSO.Sentence.Action action){ // Det her skal reworkes en del, ved ikke hvordan
        switch (action.ActionType){
            case StorySceneSO.Sentence.Action.Type.APPEAR:
                CharacterSprite.gameObject.SetActive(false);
                CharacterSprite.gameObject.SetActive(true);

                /* if (!sprites.ContainsKey(action.Speaker)){
                    controller = Instantiate(action.Speaker.prefab.gameObject, spritesPrefab.transform).GetComponent<SpriteController>();
                    sprites.Add(action.Speaker, controller);
                } else {
                    controller = sprites[action.Speaker];
                }
                controller.Setup(action.Speaker.sprites[action.SpriteIndex]);
                controller.Show(action.Coords); */
                return;
            case StorySceneSO.Sentence.Action.Type.MOVE:
                /* if (sprites.ContainsKey(action.Speaker)){
                    controller = sprites[action.Speaker];
                    controller.Move(action.Coords, action.MoveSpeed);
                    controller = sprites[action.Speaker];
                } */
                break;
            case StorySceneSO.Sentence.Action.Type.DISAPPEAR:
                CharacterSprite.GetComponent<Animator>().SetTrigger("Out");
                /* if (sprites.ContainsKey(action.Speaker)){
                    controller = sprites[action.Speaker];
                    Debug.Log(action.SpriteIndex);
                    controller.Hide();
                } */
                break;
            case StorySceneSO.Sentence.Action.Type.FLIP:
                isCharacterFlipped = !isCharacterFlipped;
                if (isCharacterFlipped){
                    CharacterSprite.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                } else {
                    CharacterSprite.gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
                /* if (sprites.ContainsKey(action.Speaker)){
                    controller = sprites[action.Speaker];
                    controller.gameObject.transform.Rotate(0, 180, 0);
                } */
                break;
            case StorySceneSO.Sentence.Action.Type.NONE:
                /* if (sprites.ContainsKey(action.Speaker)){
                    controller = sprites[action.Speaker];
                } */
                break;
        }

        /* if (controller != null){
            Debug.Log("spritecontroller wasn't null");
            if (action.ActionType == StorySceneSO.Sentence.Action.Type.DISAPPEAR) return;

            controller.SwitchSprite(action.Speaker.sprites[action.SpriteIndex]);
            Debug.Log(action.SpriteIndex);
        } */
    }

    private void PlayAudio(StorySceneSO.Sentence sentence){
        AudioManager.PlayAudio(sentence.Music, sentence.Sound);
    }

    private void PlayAudio(ChooseSceneSO chooseScene){
        AudioManager.PlayAudio(chooseScene.Music, chooseScene.Sound);
    }

    private void PlayAudio(WriteSceneSO writeScene){
        AudioManager.PlayAudio(writeScene.Music, writeScene.Sound);
    }

    public void Unpause(){
        isPaused = false;
        pauseUI.SetActive(false);
    }

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}