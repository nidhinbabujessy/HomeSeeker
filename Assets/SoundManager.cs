using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip mouseClickClip;
    public AudioClip fadeInClip;
    public AudioClip fadeOutClip;
    public AudioClip destroyClip;
    public AudioClip rockColliderClip;
    public AudioClip homeColliderClip;
    public AudioClip winClip;
    public AudioClip starClip;
    public AudioClip failClip;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";
    private const float DEFAULT_VOLUME = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_VOLUME);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM() => bgmSource?.Stop();

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayClick() => PlaySFX(mouseClickClip);
    public void PlayFadeIn() => PlaySFX(fadeInClip);
    public void PlayFadeOut() => PlaySFX(fadeOutClip);
    public void PlayDestroy() => PlaySFX(destroyClip);
    public void PlayRockCollider() => PlaySFX(rockColliderClip);
    public void PlayHomeCollider() => PlaySFX(homeColliderClip);
    public void PlayWin() => PlaySFX(winClip);
    public void PlayStar() => PlaySFX(starClip);
    public void PlayFail() => PlaySFX(failClip);

    // 🔊 Volume control
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmSource.volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxSource.volume);
        }
    }

    public float GetBGMVolume() => bgmSource != null ? bgmSource.volume : 0f;
    public float GetSFXVolume() => sfxSource != null ? sfxSource.volume : 0f;

    // 🔄 Reset to default volumes
    public void ResetVolumes()
    {
        SetBGMVolume(DEFAULT_VOLUME);
        SetSFXVolume(DEFAULT_VOLUME);
        PlayerPrefs.Save(); // Ensure changes are saved
    }
}
