using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevel = 1;
    public int score = 0;
    public int ballsCollected = 1;
    public int ballsRemaining;
    public bool isGameOver = false;

    [Header("UI References")]
    public Text scoreText;
    public Text levelText;
    public Text ballsText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public Text highScoreText;

    [Header("Game Settings")]
    public int baseScore = 10;
    public float ballRespawnDelay = 0.1f;

    [Header("References")]
    public BrickManager brickManager;
    public SlingshotController slingshotController;
    public AdManager adManager;

    private int highScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadHighScore();
        ballsRemaining = ballsCollected;
        UpdateUI();
        StartNewLevel();
    }

    public void StartNewLevel()
    {
        isGameOver = false;
        ballsRemaining = ballsCollected;
        brickManager.SpawnBricks(currentLevel);
        UpdateUI();
    }

    public void AddScore(int brickValue)
    {
        score += brickValue * baseScore;
        UpdateUI();
    }

    public void OnBallLost()
    {
        ballsRemaining--;
        UpdateUI();

        if (ballsRemaining <= 0)
        {
            // All balls lost, check if game over
            StartCoroutine(CheckGameOver());
        }
    }

    private IEnumerator CheckGameOver()
    {
        yield return new WaitForSeconds(1f);

        if (brickManager.GetLowestBrickRow() <= 0)
        {
            GameOver();
        }
        else
        {
            // Bricks move down, start new round
            brickManager.MoveBricksDown();
            ballsRemaining = ballsCollected;
            slingshotController.ResetPosition();
            UpdateUI();
        }
    }

    public void LevelComplete()
    {
        currentLevel++;
        ballsCollected++; // Reward player with extra ball each level

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // Show interstitial ad every 3 levels
        if (currentLevel % 3 == 0 && adManager != null)
        {
            adManager.ShowInterstitialAd();
        }

        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        StartNewLevel();
    }

    public void GameOver()
    {
        isGameOver = true;

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Show rewarded ad option
        if (adManager != null)
        {
            adManager.ShowRewardedAdButton();
        }
    }

    public void RestartGame()
    {
        currentLevel = 1;
        score = 0;
        ballsCollected = 1;
        ballsRemaining = 1;
        isGameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        StartNewLevel();
    }

    public void ContinueWithRewardedAd()
    {
        // Called after watching rewarded ad
        ballsRemaining = ballsCollected;
        isGameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        brickManager.MoveBricksUp(); // Give player some breathing room
        slingshotController.ResetPosition();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (levelText != null)
            levelText.text = "Level: " + currentLevel;

        if (ballsText != null)
            ballsText.text = "Balls: " + ballsRemaining + "/" + ballsCollected;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public int GetHighScore()
    {
        return highScore;
    }
}
