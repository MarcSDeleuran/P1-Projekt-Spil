using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsAndJournal : MonoBehaviour
{

    [SerializeField] public bool chapter1Completed;
    [SerializeField] public bool chapter2Completed;
    [SerializeField] public bool chapter3Completed;
    [SerializeField] public bool chapter4Completed;
    [SerializeField] public bool chapter5Completed;
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
    [SerializeField] private RectTransform stressMeter;
    [SerializeField] private GameObject[] statsButtonCharacters;
    [SerializeField] private GameObject[] statsUICharacters;
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
    public Image Vignette;
    private bool inStats;
    private int pointsRemaining = 0;
    public int schoolPoints = 100;
    public int socialPoints = 100;
    public int relaxPoints = 100;
    private bool ChangeAllowed = true;

    private void Awake()
    {
        statsButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitBeforeUpdating());
            statsUI.SetActive(true);
            GameManager.Instance.SD.VNACTIVE = false;
            // Start fra bunden
            stressSlider.value = 0;
            academicSlider.value = 0;
            socialSlider.value = 0;
            if (!chapter1Completed)
            {
                chapter1Locked.SetActive(true);
            }
            else
            {
                chapter1Locked.SetActive(false);
            }
            if (!chapter2Completed)
            {
                chapter2Locked.SetActive(true);
            }
            else
            {
                chapter2Locked.SetActive(false);
            }
            if (!chapter3Completed)
            {
                chapter3Locked.SetActive(true);
            }
            else
            {
                chapter3Locked.SetActive(false);
            }
            if (!chapter4Completed)
            {
                chapter4Locked.SetActive(true);
            }
            else
            {
                chapter4Locked.SetActive(false);
            }
            if (!chapter5Completed)
            {
                chapter5Locked.SetActive(true);
            }
            else
            {
                chapter5Locked.SetActive(false);
            }
        });
        statsBackButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SD.VNACTIVE = true;
            inStats = false;
            statsUI.SetActive(false);
        });
        journalButton.onClick.AddListener(() =>
        {
            journalUI.SetActive(true);
        });
        journalBackButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.MustAssignStats)
            {
                if (pointsRemaining <= 0)
                {
                    journalUI.SetActive(false);
                    GameManager.Instance.SD.VNACTIVE = true;
                    ChangeAllowed = false;
                }

            }
            else
                journalUI.SetActive(false);
        });
        schoolLeftButton.onClick.AddListener(() =>
        {
            if (schoolPoints >= 1 && ChangeAllowed)
            {
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                schoolPoints--;
                schoolPointsText.text = schoolPoints.ToString();
            }
        });
        schoolRightButton.onClick.AddListener(() =>
        {
            if (pointsRemaining >= 1 && ChangeAllowed)
            {
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                schoolPoints++;
                schoolPointsText.text = schoolPoints.ToString();
            }
        });
        socialLeftButton.onClick.AddListener(() =>
        {
            if (socialPoints >= 1 && ChangeAllowed)
            {
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                socialPoints--;
                socialPointsText.text = socialPoints.ToString();
            }
        });
        socialRightButton.onClick.AddListener(() =>
        {
            if (pointsRemaining >= 1 && ChangeAllowed)
            {
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                socialPoints++;
                socialPointsText.text = socialPoints.ToString();
            }
        });
        relaxLeftButton.onClick.AddListener(() =>
        {
            if (relaxPoints >= 1 && ChangeAllowed)
            {
                pointsRemaining++;
                totalPointsText.text = pointsRemaining.ToString();
                relaxPoints--;
                relaxPointsText.text = relaxPoints.ToString();
            }
        });
        relaxRightButton.onClick.AddListener(() =>
        {
            if (pointsRemaining >= 1 && ChangeAllowed)
            {
                pointsRemaining--;
                totalPointsText.text = pointsRemaining.ToString();
                relaxPoints++;
                relaxPointsText.text = relaxPoints.ToString();
            }
        });
    }

    private IEnumerator WaitBeforeUpdating()
    {
        yield return new WaitForSeconds(0.25f);

        inStats = true;
    }

    private void Update()
    {
        if (!GameManager.Instance.SD.VNACTIVE || GameManager.Instance.StressAmount < 160)
        {
            Vignette.color = new Color(1, 0, 0, 0); 
        }
        else
        {

            float stressNormalized = Mathf.Clamp01((GameManager.Instance.StressAmount - 140f) / 60f);
            float alpha = Mathf.Lerp(0, 1, stressNormalized);

            Vignette.color = new Color(1, 0, 0, alpha);
        }
        if (inStats)
        {
            float stressScore = GameManager.Instance.StressAmount; // Skal udskiftes med score score
            if (stressSlider.value != stressScore)
            {
                stressSlider.value = Mathf.Lerp(stressSlider.value, stressScore, 4f * Time.deltaTime);
            }
            float academicScore = GameManager.Instance.AcademicAmount;// Skal udskiftes med academic score
            if (academicSlider.value != academicScore)
            {
                academicSlider.value = Mathf.Lerp(academicSlider.value, academicScore, 4f * Time.deltaTime);
            }
            float socialScore = GameManager.Instance.SocialAmount;
            if (socialSlider.value != socialScore)
            {
                socialSlider.value = Mathf.Lerp(socialSlider.value, socialScore, 4f * Time.deltaTime);
            }
        }
        if (stressMeter.sizeDelta.y != GameManager.Instance.StressAmount * 1.35f)
        {
            Vector2 targetHeight = new Vector2(300, GameManager.Instance.StressAmount * 1.35f);
            float newHeightY = Mathf.Lerp(stressMeter.sizeDelta.y, targetHeight.y, 4f * Time.deltaTime);
            stressMeter.sizeDelta = new Vector2(stressMeter.sizeDelta.x, newHeightY);
            if (GameManager.Instance.StressAmount > 150)
            {
                statsButtonCharacters[0].SetActive(false);
                statsButtonCharacters[1].SetActive(false);
                statsButtonCharacters[2].SetActive(false);
                statsButtonCharacters[3].SetActive(true);

                statsUICharacters[0].SetActive(false);
                statsUICharacters[1].SetActive(false);
                statsUICharacters[2].SetActive(false);
                statsUICharacters[3].SetActive(true);
            }
            else if (GameManager.Instance.StressAmount > 100)
            {
                statsButtonCharacters[0].SetActive(false);
                statsButtonCharacters[1].SetActive(false);
                statsButtonCharacters[2].SetActive(true);
                statsButtonCharacters[3].SetActive(false);

                statsUICharacters[0].SetActive(false);
                statsUICharacters[1].SetActive(false);
                statsUICharacters[2].SetActive(true);
                statsUICharacters[3].SetActive(false);
            }
            else if (GameManager.Instance.StressAmount > 50)
            {
                statsButtonCharacters[0].SetActive(false);
                statsButtonCharacters[1].SetActive(true);
                statsButtonCharacters[2].SetActive(false);
                statsButtonCharacters[3].SetActive(false);

                statsUICharacters[0].SetActive(false);
                statsUICharacters[1].SetActive(true);
                statsUICharacters[2].SetActive(false);
                statsUICharacters[3].SetActive(false);
            }
            else
            {
                statsButtonCharacters[0].SetActive(true);
                statsButtonCharacters[1].SetActive(false);
                statsButtonCharacters[2].SetActive(false);
                statsButtonCharacters[3].SetActive(false);

                statsUICharacters[0].SetActive(true);
                statsUICharacters[1].SetActive(false);
                statsUICharacters[2].SetActive(false);
                statsUICharacters[3].SetActive(false);
            }
        }
    }
}
