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
    public int NextLevel = 1;
    public int LevelIndex = 1;

    private const string CurrentLevelKey = "CurrentLevel";
    private const string NextLevelKey = "NextLevel";
    private const string ButtonsToEnableKey = "ButtonsToEnable";

    [Header("UI Buttons")]
    [SerializeField] private Button[] levelButtons;
    public int buttonsToEnable = 0;

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

    public void ClearSavedScore()
    {
        PlayerPrefs.DeleteKey(ScoreKey);
    }

    #endregion

    #region Level Management

    IEnumerator MenuMethod()
    {
        yield return new WaitForSeconds(3);
        
        SceneManager.LoadScene("Menu");
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void LevelUpdate()
    {
        CurrentLevel++;
        NextLevel++;
        buttonsToEnable++;
        print(buttonsToEnable + "3");
        SaveLevelData();
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel);
        PlayerPrefs.SetInt(NextLevelKey, NextLevel);
        PlayerPrefs.SetInt(ButtonsToEnableKey, buttonsToEnable);
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
        PlayerPrefs.DeleteKey(CurrentLevelKey);
        PlayerPrefs.DeleteKey(NextLevelKey);
        PlayerPrefs.DeleteKey(ButtonsToEnableKey);
        print(buttonsToEnable + "5");
    } 

    #endregion
}
