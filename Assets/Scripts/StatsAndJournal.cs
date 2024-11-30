using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsAndJournal : MonoBehaviour {

    [SerializeField] private Button statsButton;
    [SerializeField] private Button statsBackButton;
    [SerializeField] private Button journalButton;
    [SerializeField] private Button journalBackButton;
    [SerializeField] private GameObject statsUI;
    [SerializeField] private GameObject journalUI;
    [SerializeField] private GameObject inGameUI;
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
            inGameUI.SetActive(false);

            // Start fra bunden
            stressSlider.value = 0;
            academicSlider.value = 0;
            socialSlider.value = 0;
        });
        statsBackButton.onClick.AddListener(() => {
            inStats = false;
            statsUI.SetActive(false);
            inGameUI.SetActive(true);
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
            float stressScore = SavedData.Instance.StressAmount; // Skal udskiftes med score score
            if (stressSlider.value != stressScore){
                stressSlider.value = Mathf.Lerp(stressSlider.value, stressScore, 4f * Time.deltaTime);
            }
            float academicScore = SavedData.Instance.AcademicAmount;// Skal udskiftes med academic score
            if (academicSlider.value != academicScore){
                academicSlider.value = Mathf.Lerp(academicSlider.value, academicScore, 4f * Time.deltaTime);
            }
            float socialScore = SavedData.Instance.SocialAmount;
            if (socialSlider.value != socialScore){
                socialSlider.value = Mathf.Lerp(socialSlider.value, socialScore, 4f * Time.deltaTime);
            }
        }
    }
}
