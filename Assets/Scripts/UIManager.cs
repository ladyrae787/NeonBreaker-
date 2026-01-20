using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("Gameplay UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text ballsText;
    [SerializeField] private Text highScoreText;

    [Header("Game Over UI")]
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text newHighScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Level Complete UI")]
    [SerializeField] private Text levelCompleteScoreText;
    [SerializeField] private Text nextLevelText;

    [Header("Settings UI")]
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle hapticsToggle;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button leaderboardButton;

    private LeaderboardManager leaderboardManager;

    private void Start()
    {
        leaderboardManager = FindObjectOfType<LeaderboardManager>();
        SetupButtons();
        LoadSettings();
        ShowMainMenu();
    }

    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ShowMainMenu);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);

        if (shopButton != null)
            shopButton.onClick.AddListener(ShowShop);

        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(ShowLeaderboard);

        // Settings toggles
        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener(OnSoundToggle);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggle);

        if (hapticsToggle != null)
            hapticsToggle.onValueChanged.AddListener(OnHapticsToggle);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChange);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        HideAllPanels();
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ShowGameOver(int finalScore, int highScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;

        if (newHighScoreText != null)
        {
            if (finalScore >= highScore)
            {
                newHighScoreText.gameObject.SetActive(true);
                newHighScoreText.text = "New High Score!";
            }
            else
            {
                newHighScoreText.gameObject.SetActive(false);
            }
        }
    }

    public void ShowLevelComplete(int level, int score)
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (levelCompleteScoreText != null)
            levelCompleteScoreText.text = "Score: " + score;

        if (nextLevelText != null)
            nextLevelText.text = "Level " + level + " Complete!";
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }

        HideAllPanels();
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        Time.timeScale = 1f;
    }

    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        LoadSettings();
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ShowShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    public void HideShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    private void LoadSettings()
    {
        if (SoundManager.Instance != null)
        {
            if (soundToggle != null)
                soundToggle.isOn = SoundManager.Instance.IsSoundEnabled();

            if (musicToggle != null)
                musicToggle.isOn = SoundManager.Instance.IsMusicEnabled();

            if (hapticsToggle != null)
                hapticsToggle.isOn = SoundManager.Instance.IsHapticsEnabled();

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = SoundManager.Instance.GetSFXVolume();

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = SoundManager.Instance.GetMusicVolume();
        }
    }

    private void OnSoundToggle(bool value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleSound();
    }

    private void OnMusicToggle(bool value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleMusic();
    }

    private void OnHapticsToggle(bool value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleHaptics();
    }

    private void OnSFXVolumeChange(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }

    private void OnMusicVolumeChange(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolume(value);
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (shopButton != null) shopButton.onClick.RemoveAllListeners();
        if (leaderboardButton != null) leaderboardButton.onClick.RemoveAllListeners();
    }
}
