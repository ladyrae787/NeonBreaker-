using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int level;
    public string date;

    public LeaderboardEntry(string name, int score, int level)
    {
        this.playerName = name;
        this.score = score;
        this.level = level;
        this.date = System.DateTime.Now.ToString("MM/dd/yyyy");
    }
}

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private int maxEntries = 10;

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private const string LEADERBOARD_KEY = "Leaderboard";

    private void Start()
    {
        LoadLeaderboard();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideLeaderboard);
        }
    }

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            DisplayLeaderboard();
        }
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void AddScore(string playerName, int score, int level)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, level);
        leaderboard.Add(newEntry);

        // Sort by score descending
        leaderboard = leaderboard.OrderByDescending(e => e.score).ToList();

        // Keep only top entries
        if (leaderboard.Count > maxEntries)
        {
            leaderboard = leaderboard.Take(maxEntries).ToList();
        }

        SaveLeaderboard();
    }

    private void DisplayLeaderboard()
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Create entry UI for each leaderboard entry
        for (int i = 0; i < leaderboard.Count; i++)
        {
            LeaderboardEntry entry = leaderboard[i];
            CreateLeaderboardEntryUI(i + 1, entry);
        }
    }

    private void CreateLeaderboardEntryUI(int rank, LeaderboardEntry entry)
    {
        if (leaderboardEntryPrefab == null || leaderboardContent == null)
            return;

        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);

        // Find and set text components
        Text[] texts = entryObj.GetComponentsInChildren<Text>();

        if (texts.Length >= 4)
        {
            texts[0].text = rank.ToString();                    // Rank
            texts[1].text = entry.playerName;                   // Name
            texts[2].text = entry.score.ToString();             // Score
            texts[3].text = $"Lvl {entry.level}";              // Level
        }

        // Highlight top 3
        if (rank <= 3)
        {
            Image bg = entryObj.GetComponent<Image>();
            if (bg != null)
            {
                if (rank == 1)
                    bg.color = new Color(1f, 0.84f, 0f, 0.3f);      // Gold
                else if (rank == 2)
                    bg.color = new Color(0.75f, 0.75f, 0.75f, 0.3f); // Silver
                else if (rank == 3)
                    bg.color = new Color(0.8f, 0.5f, 0.2f, 0.3f);   // Bronze
            }
        }
    }

    private void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { entries = leaderboard };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            leaderboard = data.entries ?? new List<LeaderboardEntry>();
        }
        else
        {
            leaderboard = new List<LeaderboardEntry>();
        }
    }

    public bool IsHighScore(int score)
    {
        if (leaderboard.Count < maxEntries)
            return true;

        return score > leaderboard[leaderboard.Count - 1].score;
    }

    public int GetRank(int score)
    {
        int rank = 1;
        foreach (var entry in leaderboard)
        {
            if (score > entry.score)
                return rank;
            rank++;
        }
        return rank;
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
}
