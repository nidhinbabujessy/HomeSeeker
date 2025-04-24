using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int score = 0;
    private const string ScoreKey = "PlayerScore";

    [Header("Level Settings")]
    public int CurrentLevel = 0;
    public int pivotCurrent = 0;
    public int pivotNext = 1;
    public int NextLevel = 1;
    public int LevelIndex = 1;

    public int PlayIndex  = 0;

    private const string CurrentLevelKey = "CurrentLevel";
    private const string PivotCurrentKey = "pivotCurrent";
    private const string PivotNextKey = "pivotNext";
    private const string NextLevelKey = "NextLevel";
    private const string ButtonsToEnableKey = "ButtonsToEnable";

    [Header("UI Buttons")]
    [SerializeField] private Button[] levelButtons;
    public int buttonsToEnable = 0;

    public Slider loadingSlider; // Assign via Inspector



    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        print(buttonsToEnable + "1");
       // LevelUpdate();
        LoadScore();
        LoadLevelData();

    }

    private void Start()
    {
        Debug.Log("GameManager Started in Scene: " + SceneManager.GetActiveScene().name);
        print(buttonsToEnable+"1");
        StartCoroutine(MenuMethod());

        //volume

    }

    #region Score Management

    public void AddScore(int value)
    {
        score += value;
        SaveScore();
        Debug.Log("Score Added: " + value);
    }

    public void ResetScore()
    {
        score = 0;
        SaveScore();
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        score = PlayerPrefs.GetInt(ScoreKey, 0);
    }

    void ClearSavedScore()
    {
        PlayerPrefs.DeleteKey(ScoreKey);
    }

    #endregion

    #region Level Management

    IEnumerator MenuMethod()
    {
        float duration = 3f;
        float elapsed = 0f;

        // Reset slider
        if (loadingSlider != null)
            loadingSlider.value = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (loadingSlider != null)
                loadingSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        SceneManager.LoadScene("Menu");
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void LevelUpdate()
    {
        printlevel();
        if (pivotCurrent == LevelIndex)
        {
            pivotCurrent++;
            pivotNext++;
        }
        else
        {
            CurrentLevel++;
            NextLevel++;

            buttonsToEnable++;
        }
        print(buttonsToEnable + "3");
        SaveLevelData();
        printlevel();
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel);
        PlayerPrefs.SetInt(NextLevelKey, NextLevel);
        PlayerPrefs.SetInt(ButtonsToEnableKey, buttonsToEnable);
        PlayerPrefs.SetInt(PivotCurrentKey, pivotCurrent);
        PlayerPrefs.SetInt(PivotNextKey, pivotNext);
        print(buttonsToEnable + "4");
        PlayerPrefs.Save();
    }

    private void LoadLevelData()
    {
        if (PlayerPrefs.HasKey(ButtonsToEnableKey))
        {
            buttonsToEnable = PlayerPrefs.GetInt(ButtonsToEnableKey);
        }
        else
        {
            buttonsToEnable = 1; // Set initial default value
        }

        CurrentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 0);
        NextLevel = PlayerPrefs.GetInt(NextLevelKey, 1);

        print(buttonsToEnable + "2");
    }


    public void ClearSavedLevelData()
    {
        ClearSavedScore();
        SoundManager.Instance.ResetVolumes();
        PlayerPrefs.DeleteKey(CurrentLevelKey);
        PlayerPrefs.DeleteKey(NextLevelKey);
        PlayerPrefs.DeleteKey(ButtonsToEnableKey);

        PlayerPrefs.DeleteKey(PivotCurrentKey);
        PlayerPrefs.DeleteKey(PivotNextKey);


        buttonsToEnable = 1;
        print(buttonsToEnable + "5");

        CurrentLevel = 0;
        NextLevel = 1;
        LevelIndex = 1;

        pivotCurrent = 0;
        pivotNext = 1;
        PlayerPrefs.Save();
    }


    public void printlevel()
    {
        print("current l  "+CurrentLevel);
        print("next l  " + NextLevel);
        print("pivot current l  " + pivotCurrent);
        print("pivot next l  " + pivotNext);
        print("level index l  " + LevelIndex);

      
    }
    // random level load

    #endregion

    public void timeStop()
    {
        Time.timeScale = 0f;
    }
    public void timeStart()
    {
        Time.timeScale = 1f;
    }
}
