using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;

    private const string ScoreKey = "PlayerScore";

    public int CurrentLevel = 0;
    public int NextLevel = 1;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes

            LoadScore(); // Load score on start
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method to increase the score


    #region Score
    public void AddScore(int value)
    {
        score += value;
        SaveScore();
        print("score added");
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

    // Optional: Call this to clear saved data
    public void ClearSavedScore()
    {
        PlayerPrefs.DeleteKey(ScoreKey);
    }
    #endregion

    #region level
    public void level1()
    {
        SceneManager.LoadScene("SampleScene");
        //GameMan.instance.levelNext();
      //  GameMan.instance.loadlevelfromHome();
    } 
    public void Home()
    {
        SceneManager.LoadScene("Home");
    }
    public void LevelUpdate()
    {
        CurrentLevel++;
        NextLevel++;
    }
    #endregion
}
