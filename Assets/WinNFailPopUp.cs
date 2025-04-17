using UnityEngine;
using TMPro; // Don't forget this if you're using TMP

public class WinNFailPopUp : MonoBehaviour
{
    public static WinNFailPopUp Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    int time;
    int currenttime;

    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject scoreText; // Changed from GameObject to TMP_Text
    [SerializeField] private GameObject highscoreText;
    [SerializeField] private TMP_Text score;
    
    public int scoreEarned;

    public void StarEnable()
    {
        // Get values from GameMan (assuming it's a singleton class like GameManager)
        time = GameMan.instance.Time;
        currenttime = GameMan.instance.currentTime;

        timeDivision();
    }

    void timeDivision()
    {
        int divisionStrength = time / 3;
        int firstD = time - (divisionStrength * 2);
        int secondD = time - divisionStrength;

        if (currenttime < time&&currenttime>secondD)
        {
            Debug.Log("3 stars");
            ActivateStars(3);
            scoreEarned = 100;
        }
        else if (currenttime < secondD&&currenttime>firstD)
        {
            Debug.Log("2 stars");
            ActivateStars(2);
            scoreEarned = 50;
        }
        else if (currenttime <= firstD)
        {
            Debug.Log("1 star");
            ActivateStars(1);
            scoreEarned = 25;
        }
        score.text=scoreEarned.ToString();
       
        Debug.Log("Earned Score = " + scoreEarned);
        FlipGameObject(scoreText);
        FlipGameObject(highscoreText);
      
    }

    void ActivateStars(int count)
    {
        for (int i = 0; i < count; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void winReset()
    {
        // 1. Disable all stars
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }

        // 2. Reset score
        scoreEarned = 0;

        //// 3. Reset score text (if TMP_Text)
        //TMP_Text scoreTMP = scoreText.GetComponent<TMP_Text>();
        //if (scoreTMP != null)
        //{
        //    scoreTMP.text = "Score: 0";
        //}

        // 4. Optionally reset time variables
        //time = 0;
        //currenttime = 0;

        // 5. Cancel any LeanTweens (optional but good for safety)
        LeanTween.cancel(scoreText);
    }


    void FlipGameObject(GameObject obj)
    {
        if (obj == null) return;

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) rect = obj.AddComponent<RectTransform>(); // just in case (for UI), or use Transform

        // Flip 90°, change state, flip back
        LeanTween.rotateY(obj, 90f, 0.2f).setOnComplete(() =>
        {
            // Place your logic here, e.g., change sprite, material, etc.
            Debug.Log("Halfway through flip");

            LeanTween.rotateY(obj, 0f, 0.2f);
        });
    }

}
