using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("UI Buttons")]
    [SerializeField] private Button[] levelButtons; // Assign buttons in Inspector
     int buttonsstrength ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       buttonsstrength= GameManager.Instance.buttonsToEnable;
        EnableButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LevelButtonClicked()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (clickedButton != null)
        {
            string tagValue = clickedButton.tag;

            if (int.TryParse(tagValue, out int levelIndex))
            {
                if(GameManager.Instance.pivotCurrent<GameManager.Instance.CurrentLevel)
                {
                    GameManager.Instance.pivotCurrent=levelIndex;
                    GameManager.Instance.pivotNext=levelIndex+1;
                }
                Debug.Log("Clicked Level Tag: " + levelIndex);
               GameManager.Instance.LevelIndex = levelIndex;
                StartCoroutine(WaitAndLoadLevel());
                GameManager.Instance.printlevel();
            }
            else
            {
                Debug.LogWarning("Button tag is not a valid level index.");
            }
        }
        else
        {
            Debug.LogWarning("No button was clicked.");
        }
    }

    IEnumerator WaitAndLoadLevel()
    {
        yield return new WaitForSeconds(2f);

        // You can replace this with a dynamic level name like "Level_1", "Level_2", etc.
        SceneManager.LoadScene("SampleScene");
    }

    void EnableButtons()
    {
        for (int i = 0; i < buttonsstrength; i++)
        {
            levelButtons[i].interactable = true;
        }
    }


    //============= Reset all the Game=======
    public void ResetGame()
    {
        GameManager.Instance.ClearSavedLevelData();
        SceneManager.LoadScene("Menu");
    }
}
 