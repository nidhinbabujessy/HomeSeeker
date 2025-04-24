using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameMan : MonoBehaviour
{
    public static GameMan instance {  get; private set; }


    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private GameObject pathPrefab;
   // [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject firstObject;
    [SerializeField] private GameObject lastObjectWin;
    [SerializeField] private GameObject borderObject;



    [SerializeField] private float spacing = 1.1f;
    [SerializeField] private Vector2 squareSize = new Vector2(1f, 1f);
    [SerializeField] private Transform parent;
    [SerializeField] private int currentLevel = 1;

    [Header("Win")]
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;

    [Header("Timer ")]
    [SerializeField] private TMP_Text TimerText;
    public int Time=15;

    [Header("Timer ")]
    [SerializeField] private TMP_Text Scoretext;

    [Header("Exit")]
    [SerializeField] private GameObject exit;

    [Header("Score ")]
    public int score;
    [SerializeField] private TMP_Text Highscore;

    //SLPASH ANIMATION
    public Transform target;
    public Transform target2;
    [SerializeField] private GameObject SPASH;
    [SerializeField] private GameObject SPASHPARENT;

    private int rows = 5;
    private int columns = 5;
    private List<PathTile> currentPathTiles;

    private ObjectPoolScript squarePool;
    private ObjectPoolScript pathPool;

    private void Awake()
    {
        squarePool = new ObjectPoolScript(squarePrefab, 50);
        pathPool = new ObjectPoolScript(pathPrefab, 50);

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
      
        // levelNext();
        int index = GameManager.Instance.LevelIndex;
        print(index);
        LoadLevel(index);
        winSpawn();
        loadScore();
        ResetGame();
        SPASHPARENT.SetActive(true);
        StartCoroutine(WaitAndLoadLevel());
        

    }

    #region splash animation
    IEnumerator WaitAndLoadLevel()
    {
       
        ThrowBall(SPASH,SPASHPARENT);
        yield return new WaitForSeconds(2f);

     //   SPASHPARENT.SetActive(false);
        // You can replace this with a dynamic level name like "Level_1", "Level_2", etc.

    }
    // animation
    void ThrowBall(GameObject obj, GameObject obj2)
    {
        SoundManager.Instance.PlayFadeOut();
        float duration = 1f;

        // Move toward the wall
        LeanTween.move(obj, target.position, duration)
                 .setEase(LeanTweenType.easeInExpo);

        // Fade out at the same time
        LeanTween.value(obj, 1f, 0f, duration).setOnUpdate((float val) =>
        {
            SetObjectAlpha(obj, val);
        });
        LeanTween.value(obj2, 1f, 0f, duration).setOnUpdate((float val) =>
        {
            SetObjectAlpha(obj, val);
        });
    }


    void SetObjectAlpha(GameObject obj, float alpha)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        var img = obj.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }

    IEnumerator StartAnimationAndLoadScene()
    {
        ForWardThrowBall(SPASH);
        yield return new WaitForSeconds(2f); // Let the animation play
        win.SetActive(false);
        lose.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }

    void ForWardThrowBall(GameObject obj)
    {
        SoundManager.Instance.PlayFadeIn();
        SPASH.SetActive(true);
        float fadeDuration = 0.5f;
        float moveDuration = 1f;

        // Set initial alpha to 0 (fully transparent)
        SetObjectAlpha(obj, 0f);

        // Fade in
        LeanTween.value(obj, 0f, 1f, fadeDuration).setOnUpdate((float val) =>
        {
            SetObjectAlpha(obj, val);
        }).setOnComplete(() =>
        {
            // Move after fade-in completes
            LeanTween.move(obj, target2.position, moveDuration)
                     .setEase(LeanTweenType.easeInExpo);
        });
    }


    #endregion

    #region LEVEL DESIGN

    void winSpawn()
    {
        Vector2 scaleWin = new Vector2(0.07767631f, 0.07922984f);
        GameObject lastObject = Instantiate(lastObjectWin);
        lastObject.transform.localScale = scaleWin;
        print("winspawn");
    }
    private void UpdateGridSize(int level)
    {
        int group = (level - 1) / 5;
        int gridSize = 5 + group;
        rows = gridSize;
        columns = gridSize;
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        UpdateGridSize(level);
        LoadLevelData(level);
        CenterParent();
        SpawnGrid();
        loadScore();

    }

    private void LoadLevelData(int level)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Levels");
        if (jsonFile == null)
        {
            Debug.LogError("Levels.json not found in Resources folder!");
            return;
        }

        LevelCollection levelCollection = JsonUtility.FromJson<LevelCollection>(jsonFile.text);

        LevelData selectedLevel = levelCollection.levels.Find(l => l.level == level);
        if (selectedLevel != null)
        {
            currentPathTiles = selectedLevel.paths;
        }
        else
        {
            Debug.LogWarning("Level " + level + " not found, defaulting to empty path.");
            currentPathTiles = new List<PathTile>();
        }
    }

    private void CenterParent()
    {
        float gridWidth = (columns - 1) * spacing;
        float gridHeight = (rows - 1) * spacing;
        Vector2 centerOffset = new Vector2(-gridWidth / 2f, gridHeight / 2f);
        parent.transform.position = centerOffset;
    }

    private void SpawnGrid()
    {
        // Return all existing tiles to pool
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Square"))
                squarePool.ReturnToPool(child.gameObject);
            else if (child.CompareTag("Path"))
                pathPool.ReturnToPool(child.gameObject);
            else if (child.CompareTag("Border"))
                Destroy(child.gameObject); // Assuming no pool for borders
        }

        GameObject firstTileObject = null;
        GameObject lastTileObject = null;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 spawnPos = new Vector2(
                    parent.transform.position.x + col * spacing,
                    parent.transform.position.y - row * spacing
                );

                // Check if tile is on the border (outer edge)
                bool isBorder = row == 0 || row == rows - 1 || col == 0 || col == columns - 1;

                // Check if it's a path tile
                bool isPath = currentPathTiles.Exists(p => p.row == row && p.col == col);

                GameObject tile;

                if (isBorder)
                {
                    tile = Instantiate(borderObject, spawnPos, Quaternion.identity, parent);
                    tile.transform.localScale = squareSize;
                    tile.tag = "Border"; // Set this in Unity tag manager too
                    continue; // Skip placing square or path if it's a border
                }

                tile = isPath
                    ? pathPool.Get(spawnPos, Quaternion.identity, parent)
                    : squarePool.Get(spawnPos, Quaternion.identity, parent);

                tile.transform.localScale = squareSize;

                // Store the first and last path tiles
                if (isPath)
                {
                    PathTile currentTile = currentPathTiles.Find(p => p.row == row && p.col == col);
                    if (currentTile.Equals(currentPathTiles[0]))
                    {
                        firstTileObject = tile;
                    }
                    if (currentTile.Equals(currentPathTiles[currentPathTiles.Count - 1]))
                    {
                        lastTileObject = tile;

                    }

                    Debug.Log("Path tile at: " + row + "," + col);
                }
            }
        }

       

        if (firstTileObject != null)
        {
            firstObject.SetActive(true);
            firstObject.transform.position = firstTileObject.transform.position;
            Ball.Instance.InitialStage();

      

        }


        if (lastTileObject != null)
        {
            lastObjectWin.SetActive(true);
            lastObjectWin.transform.position = lastTileObject.transform.position;
        }
    }

    #endregion

    #region Level


    public void levelNext()
    {
        SoundManager.Instance.PlayClick();

        if (GameManager.Instance.LevelIndex == GameManager.Instance.CurrentLevel)
        {
            GameManager.Instance.LevelIndex = GameManager.Instance.NextLevel;
        }
        else
        {
            GameManager.Instance.LevelIndex = GameManager.Instance.pivotNext;
            print("level index = pivot next");
        }

        GameManager.Instance.printlevel();

        StartCoroutine(StartAnimationAndLoadScene());
    }

    public void RetryLevel()
    {
        SoundManager.Instance.PlayClick();
        if (GameManager.Instance.LevelIndex == GameManager.Instance.CurrentLevel)
        {
            GameManager.Instance.LevelIndex = GameManager.Instance.CurrentLevel;
           
        }
        else
        {
            GameManager.Instance.LevelIndex = GameManager.Instance.pivotCurrent;
        }
        GameManager.Instance.printlevel();
        StartCoroutine(StartAnimationAndLoadScene());
    }
    public void homeScene()
    {
        SoundManager.Instance.PlayClick();
        GameManager.Instance.timeStart();
        SceneManager.LoadScene("Menu");
    }





    #endregion


    #region reset
    void ResetGame()
    {

       // Ball.Instance.startstage();
        win.SetActive(false);
        lose.SetActive(false);
        // Destroy(lastObjectWin);
        WinNFailPopUp.Instance.winReset();

        //time
        currentTime=0;
    }
    #endregion


    #region WIN

    public void winMethod()
    {
        // ResetGame();
        SoundManager.Instance.PlayWin();
        win.SetActive(true);
        WinNFailPopUp.Instance.StarEnable();
        GameManager.Instance.AddScore(WinNFailPopUp.Instance.scoreEarned);
        loadScore();
       if (GameManager.Instance.NextLevel == GameManager.Instance.LevelIndex)
        {
            print(GameManager.Instance.NextLevel + "---");
            GameManager.Instance.LevelUpdate();
        }
      

    }
    #endregion


    #region Timer

    // call this method in ball script
    public int currentTime;
    private Coroutine timerCoroutine;

    public void TimeStart()
    {
        currentTime = Time;
        UpdateTimerText();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime--;
            UpdateTimerText();
        }

        TimerEnd(); // Optional: Handle when timer reaches 0
    }

    private void UpdateTimerText()
    {
        TimerText.text = currentTime.ToString("D2");
    }

    //=============================================FAIL====================
    private void TimerEnd()
    {
        Debug.Log("⏰ Time's up!");
        lose.SetActive(true );
        SoundManager.Instance.PlayFail();        // Add game over, next level, or popup logic here
    }
    //==================================================================
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    #endregion

    #region Score

    void loadScore()
    {
        score= GameManager.Instance.score;
        Scoretext.text=score.ToString();
        Highscore.text=score.ToString();
        print("score loaded");
    }

    #endregion


    #region Exit

    public void exitEnable()
    {
        SoundManager.Instance.PlayClick();
        exit.SetActive(true );
        GameManager.Instance.timeStop();
    }
    public void exitdisable()
    {
        SoundManager.Instance.PlayClick();
        GameManager.Instance.timeStart();
        exit.SetActive(false);
    }
    #endregion
} 
