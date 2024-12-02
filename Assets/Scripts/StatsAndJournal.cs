using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsAndJournal : MonoBehaviour {

    [SerializeField] private bool chapter1Completed;
    [SerializeField] private bool chapter2Completed;
    [SerializeField] private bool chapter3Completed;
    [SerializeField] private bool chapter4Completed;
    [SerializeField] private bool chapter5Completed;
    [Space(5)]
    [SerializeField] private GameObject chapter1Locked;
    [SerializeField] private GameObject chapter2Locked;
    [SerializeField] private GameObject chapter3Locked;
    [SerializeField] private GameObject chapter4Locked;
    [SerializeField] private GameObject chapter5Locked;
    [Space(5)]
    [SerializeField] private Button statsButton;
    [SerializeField] private Button statsBackButton;
    [SerializeField] private Button journalButton;
    [SerializeField] private Button journalBackButton;
    [SerializeField] private GameObject statsUI;
    [SerializeField] private GameObject journalUI;
    [Space(10)]
    [SerializeField] private Slider stressSlider;
    [SerializeField] private Slider academicSlider;
    [SerializeField] private Slider socialSlider;
    [Space(10)]
    // Der er selvfølgelig en simplere måde at gøre dette på
    [SerializeField] private TextMeshProUGUI totalPointsText;
    [Space(5)]
    [SerializeField] private Button schoolLeftButton;
    [SerializeField] private Button schoolRightButton;
    [SerializeField] private TextMeshProUGUI schoolPointsText;
    [Space(5)]
    [SerializeField] private Button socialLeftButton;
    [SerializeField] private Button socialRightButton;
    [SerializeField] private TextMeshProUGUI socialPointsText;
    [Space(5)]
    [SerializeField] private Button relaxLeftButton;
    [SerializeField] private Button relaxRightButton;
    [SerializeField] private TextMeshProUGUI relaxPointsText;
    private bool inStats;
    private int pointsRemaining = 10;
    private int schoolPoints;
    private int socialPoints;
    private int relaxPoints;

    private void Awake(){
        statsButton.onClick.AddListener(() => {
            StartCoroutine(WaitBeforeUpdating());
            statsUI.SetActive(true);
            GameManager.Instance.SD.VNACTIVE = false;
            // Start fra bunden
            stressSlider.value = 0;
            academicSlider.value = 0;
            socialSlider.value = 0;
            if (!chapter1Completed){
                chapter1Locked.SetActive(true);
            }
            if (!chapter2Completed){
                chapter2Locked.SetActive(true);
            }
            if (!chapter3Completed){
                chapter3Locked.SetActive(true);
            }
            if (!chapter4Completed){
                chapter4Locked.SetActive(true);
            }
            if (!chapter5Completed){
                chapter5Locked.SetActive(true);
            }
        });
        statsBackButton.onClick.AddListener(() => {
            GameManager.Instance.SD.VNACTIVE = true;
            inStats = false;
            statsUI.SetActive(false);
        });
        journalButton.onClick.AddListener(() => {
            journalUI.SetActive(true);
        });
        journalBackButton.onClick.AddListener(() => {
            journalUI.SetActive(false);
        });
        schoolLeftButton.onClick.AddListener(() => {
            if (schoolPoints >= 1){
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                schoolPoints--;
                schoolPointsText.text = schoolPoints.ToString();
            }
        });
        schoolRightButton.onClick.AddListener(() => {
            if (pointsRemaining >= 1){
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                schoolPoints++;
                schoolPointsText.text = schoolPoints.ToString();
            }
        });
        socialLeftButton.onClick.AddListener(() => {
            if (socialPoints >= 1){
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                socialPoints--;
                socialPointsText.text = socialPoints.ToString();
            }
        });
        socialRightButton.onClick.AddListener(() => {
            if (pointsRemaining >= 1){
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                socialPoints++;
                socialPointsText.text = socialPoints.ToString();
            }
        });
        relaxLeftButton.onClick.AddListener(() => {
            if (relaxPoints >= 1){
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                relaxPoints--;
                relaxPointsText.text = relaxPoints.ToString();
            }
        });
        relaxRightButton.onClick.AddListener(() => {
            if (pointsRemaining >= 1){
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                relaxPoints++;
                relaxPointsText.text = relaxPoints.ToString();
            }
        });
    }

    private IEnumerator WaitBeforeUpdating(){
        yield return new WaitForSeconds(0.25f);

        inStats = true;
    }

    private void Update(){
        if (inStats){
            float stressScore = GameManager.Instance.StressAmount; // Skal udskiftes med score score
            if (stressSlider.value != stressScore){
                stressSlider.value = Mathf.Lerp(stressSlider.value, stressScore, 4f * Time.deltaTime);
            }
            float academicScore = GameManager.Instance.AcademicAmount;// Skal udskiftes med academic score
            if (academicSlider.value != academicScore){
                academicSlider.value = Mathf.Lerp(academicSlider.value, academicScore, 4f * Time.deltaTime);
            }
            float socialScore = GameManager.Instance.SocialAmount;
            if (socialSlider.value != socialScore){
                socialSlider.value = Mathf.Lerp(socialSlider.value, socialScore, 4f * Time.deltaTime);
            }
        }
    }
}
