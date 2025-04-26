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

    [Header("Music")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        parentSplash.SetActive(false);
        buttonsstrength = GameManager.Instance.buttonsToEnable;
        EnableButtons();

        // Volume
        // Set initial slider values from SoundManager
        if (SoundManager.Instance != null)
        {
            bgmSlider.value = SoundManager.Instance.GetBGMVolume();
            sfxSlider.value = SoundManager.Instance.GetSFXVolume();
        }

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Update is called once per frame
    void Update()
    {
        score.text = GameManager.Instance.score.ToString();
    }

    #region Music
    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance?.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance?.SetSFXVolume(volume);
    }

    #endregion



    #region Level
    public void LevelButtonClicked()
    {
        SoundManager.Instance.PlayClick();

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (clickedButton != null)
        {
            ButtonIndex buttonIndex = clickedButton.GetComponent<ButtonIndex>();

            if (buttonIndex != null)
            {
                int levelIndex = buttonIndex.index;

                if (GameManager.Instance.pivotCurrent < GameManager.Instance.CurrentLevel)
                {
                    GameManager.Instance.pivotCurrent = levelIndex-1;
                    GameManager.Instance.pivotNext = levelIndex ;
                }

                Debug.Log("Clicked Level Index: " + levelIndex);
                GameManager.Instance.LevelIndex = levelIndex;
                StartCoroutine(WaitAndLoadLevel());
                GameManager.Instance.printlevel();
            }
            else
            {
                Debug.LogWarning("Button does not have ButtonIndex component.");
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
        SoundManager.Instance.PlayFadeIn();
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
        SoundManager.Instance.PlayClick();
        if (GameManager.Instance.LevelIndex>GameManager.Instance.PlayIndex)
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
            print(" EnableButtons = " + buttonsstrength);
        }
    }


    //============= Reset all the Game=======
    public void ResetGame()
    {
        SoundManager.Instance.PlayClick();
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
        SoundManager.Instance.PlayClick();
        settings.SetActive(true);
    }

    public void SettingsDisable()
    {
        SoundManager.Instance.PlayClick();
        settings.SetActive(false);
    }

    #endregion
}
