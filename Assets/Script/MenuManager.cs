using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MenuManager : MonoBehaviour
{

    [Header("UI Buttons")]
    [SerializeField] private Button[] levelButtons; // Assign buttons in Inspector

    [SerializeField] private TMP_Text score;
     int buttonsstrength ;
    [Header("Settings")]
    [SerializeField] private GameObject settings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Animation")]
    //Animation
    public Transform target;  // Wall or target position
  
    [SerializeField] private GameObject parentSplash;
    [SerializeField] private GameObject AniamtedObject;

    void Start()
    {
        parentSplash.SetActive(false);
        buttonsstrength = GameManager.Instance.buttonsToEnable;
        EnableButtons();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = GameManager.Instance.score.ToString();
    }

    #region Level
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
        parentSplash.SetActive(true);
        ThrowBall(AniamtedObject);
        yield return new WaitForSeconds(2f);

       
        // You can replace this with a dynamic level name like "Level_1", "Level_2", etc.
        SceneManager.LoadScene("SampleScene");
    }

    // animation
    void ThrowBall(GameObject obj)
    {
        float fadeDuration = 0.5f;
        float moveDuration = 1f;

        Image img = obj.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("Missing Image component on object: " + obj.name);
            return;
        }

        // Set initial transparency
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        // Fade in
        LeanTween.value(obj, 0f, 1f, fadeDuration).setOnUpdate((float val) =>
        {
            Color newColor = img.color;
            newColor.a = val;
            img.color = newColor;
        }).setOnComplete(() =>
        {
            // Move after fade-in
            LeanTween.move(obj, target.position, moveDuration)
                     .setEase(LeanTweenType.easeInExpo);
        });
    }



    public void Play()
    {
        if(GameManager.Instance.LevelIndex>GameManager.Instance.PlayIndex)
        {
            GameManager.Instance.PlayIndex=GameManager.Instance.LevelIndex;
        }
        GameManager.Instance.LevelIndex=GameManager.Instance.LevelIndex;
        SceneManager.LoadScene("SampleScene");

    }

    #endregion
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
    //====================

    public ScrollRect scrollRect;

    public void OnDrag(PointerEventData eventData)
    {
        // Reverse direction for natural feel
        scrollRect.content.anchoredPosition += new Vector2( eventData.delta.x,0);
    }

    #region Settings

    public void SettingssEnable()
    {
        settings.SetActive(true);
    }

    public void SettingsDisable()
    {
        settings.SetActive(false);
    }

    #endregion
}
