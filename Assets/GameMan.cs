using UnityEngine;
using System.Collections.Generic;

public class GameMan : MonoBehaviour
{
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

    private int rows = 5;
    private int columns = 5;
    private List<PathTile> currentPathTiles;

    private ObjectPoolScript squarePool;
    private ObjectPoolScript pathPool;

    private void Awake()
    {
        squarePool = new ObjectPoolScript(squarePrefab, 50);
        pathPool = new ObjectPoolScript(pathPrefab, 50);
    }

    private void Start()
    {
        LoadLevel(currentLevel);
        winSpawn();

    }

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

        // Place first and last objects
        if (firstTileObject != null)
        {
            firstObject.SetActive(true);
            firstObject.transform.position = firstTileObject.transform.position;
        }

        if (lastTileObject != null)
        {
            lastObjectWin.SetActive(true);
            lastObjectWin.transform.position = lastTileObject.transform.position;
        }
    }



    public void level2()
    {
        ResetGame();
        LoadLevel(2);
        
    }

    #endregion


    #region reset
    void ResetGame()
    {
        Ball.Instance.startstage();
        // Destroy(lastObjectWin);
    }
    #endregion
}
