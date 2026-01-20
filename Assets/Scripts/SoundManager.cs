using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip ballLaunchSound;
    [SerializeField] private AudioClip ballBounceSound;
    [SerializeField] private AudioClip brickHitSound;
    [SerializeField] private AudioClip brickDestroySound;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip powerUpSound;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Settings")]
    [SerializeField] private bool soundEnabled = true;
    [SerializeField] private bool musicEnabled = true;
    [SerializeField] private bool hapticsEnabled = true;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 0.5f;

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
            return;
        }

        InitializeAudio();
        LoadSettings();
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void InitializeAudio()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void LoadSettings()
    {
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        hapticsEnabled = PlayerPrefs.GetInt("HapticsEnabled", 1) == 1;
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        ApplySettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("HapticsEnabled", hapticsEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.mute = !musicEnabled;
        }
    }

    // ===== PLAY SOUNDS =====
    public void PlayBallLaunch()
    {
        PlaySound(ballLaunchSound);
        VibrateLight();
    }

    public void PlayBallBounce()
    {
        PlaySound(ballBounceSound, 0.3f);
    }

    public void PlayBrickHit()
    {
        PlaySound(brickHitSound, 0.6f);
        VibrateLight();
    }

    public void PlayBrickDestroy()
    {
        PlaySound(brickDestroySound);
        VibrateMedium();
    }

    public void PlayLevelComplete()
    {
        PlaySound(levelCompleteSound);
        VibrateHeavy();
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
        VibrateHeavy();
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
        VibrateLight();
    }

    public void PlayPowerUp()
    {
        PlaySound(powerUpSound);
        VibrateMedium();
    }

    private void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || !soundEnabled || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // ===== BACKGROUND MUSIC =====
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicEnabled && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // ===== HAPTICS =====
    private void VibrateLight()
    {
        if (!hapticsEnabled) return;

        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }

    private void VibrateMedium()
    {
        if (!hapticsEnabled) return;

        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }

    private void VibrateHeavy()
    {
        if (!hapticsEnabled) return;

        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }

    // ===== SETTINGS =====
    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        SaveSettings();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        ApplySettings();
        SaveSettings();

        if (musicEnabled)
            PlayBackgroundMusic();
        else
            StopBackgroundMusic();
    }

    public void ToggleHaptics()
    {
        hapticsEnabled = !hapticsEnabled;
        SaveSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplySettings();
        SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplySettings();
        SaveSettings();
    }

    public bool IsSoundEnabled() => soundEnabled;
    public bool IsMusicEnabled() => musicEnabled;
    public bool IsHapticsEnabled() => hapticsEnabled;
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
}
