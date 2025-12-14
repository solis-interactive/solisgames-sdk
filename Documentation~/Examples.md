# Code Examples

Complete code examples for common use cases with the Solis Games Unity SDK.

---

## Table of Contents

1. [Basic Integration](#basic-integration)
2. [User Profile Display](#user-profile-display)
3. [Leaderboard System](#leaderboard-system)
4. [Cloud Save Manager](#cloud-save-manager)
5. [Achievement Tracker](#achievement-tracker)
6. [Tournament UI](#tournament-ui)
7. [Rewarded Ads](#rewarded-ads)
8. [Friends List](#friends-list)
9. [In-Game Chat](#in-game-chat)
10. [Complete Game Manager](#complete-game-manager)

---

## Basic Integration

Minimal setup to get started:

```csharp
using UnityEngine;
using SolisGames;

public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        // Initialize SDK
        bool success = await SolisSDK.InitAsync("your-api-key-here");

        if (success)
        {
            Debug.Log("✅ Solis Games SDK initialized!");

            // Get user data
            var user = await SolisSDK.User.GetUserAsync();
            Debug.Log($"Welcome, {user.username}!");

            // Track game start
            SolisSDK.Analytics.TrackEvent("game_started");
        }
        else
        {
            Debug.LogError("❌ SDK initialization failed");
        }
    }
}
```

---

## User Profile Display

Display player profile with avatar and stats:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;

public class PlayerProfileUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI emailText;
    public TextMeshProUGUI levelText;
    public Image avatarImage;
    public GameObject premiumBadge;

    async void Start()
    {
        await LoadPlayerProfile();
    }

    async Task LoadPlayerProfile()
    {
        try
        {
            var user = await SolisSDK.User.GetUserAsync();

            // Display username
            usernameText.text = user.username;
            emailText.text = user.email;

            // Get achievement stats for level
            var stats = await SolisSDK.Achievements.GetStatsAsync();
            levelText.text = $"Level {stats.level}";

            // Show premium badge if user is premium
            premiumBadge.SetActive(user.isPremium);

            // Load avatar (if you have a URL)
            if (!string.IsNullOrEmpty(user.avatarUrl))
            {
                await LoadAvatarImage(user.avatarUrl);
            }

            Debug.Log($"Profile loaded for {user.username}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load profile: {ex.Message}");
            ShowErrorMessage("Could not load your profile");
        }
    }

    async Task LoadAvatarImage(string url)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            await www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
                avatarImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }
    }

    void ShowErrorMessage(string message)
    {
        // Display error to player
        Debug.LogWarning(message);
    }
}
```

---

## Leaderboard System

Complete leaderboard with submit, display, and realtime updates:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;
    public TextMeshProUGUI yourRankText;
    public Button refreshButton;

    [Header("Settings")]
    public string leaderboardKey = "high_scores";
    public int entriesPerPage = 100;

    private float currentScore;

    void Start()
    {
        refreshButton.onClick.AddListener(RefreshLeaderboard);
        RefreshLeaderboard();
    }

    // Call this when player finishes game
    public async void SubmitScore(float score)
    {
        currentScore = score;

        try
        {
            var result = await SolisSDK.Leaderboards.SubmitAsync(
                leaderboardKey,
                score,
                new Dictionary<string, object>
                {
                    { "timestamp", System.DateTime.UtcNow.ToString() }
                }
            );

            if (result.success)
            {
                yourRankText.text = $"Your Rank: #{result.rank}";
                Debug.Log($"Score submitted! Rank: #{result.rank}");

                // Check for anti-cheat flags
                if (result.flaggedForReview)
                {
                    Debug.LogWarning($"Score flagged: {string.Join(", ", result.flags)}");
                }

                // Refresh leaderboard to show updated rankings
                RefreshLeaderboard();
            }
            else
            {
                ShowErrorMessage("Failed to submit score. Please try again.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Submit score failed: {ex.Message}");
            ShowErrorMessage("Could not submit score");
        }
    }

    public async void RefreshLeaderboard()
    {
        try
        {
            refreshButton.interactable = false;

            // Fetch top rankings
            var rankings = await SolisSDK.Leaderboards.GetAsync(
                leaderboardKey,
                scope: "all_time",
                limit: entriesPerPage
            );

            // Clear existing entries
            foreach (Transform child in leaderboardContainer)
            {
                Destroy(child.gameObject);
            }

            // Create UI for each entry
            foreach (var entry in rankings.entries)
            {
                CreateLeaderboardEntry(entry);
            }

            Debug.Log($"Leaderboard refreshed: {rankings.entries.Count} entries");

            refreshButton.interactable = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Refresh leaderboard failed: {ex.Message}");
            refreshButton.interactable = true;
        }
    }

    void CreateLeaderboardEntry(LeaderboardEntry entry)
    {
        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);

        var rankText = entryObj.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
        var nameText = entryObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        var scoreText = entryObj.transform.Find("Score").GetComponent<TextMeshProUGUI>();

        rankText.text = $"#{entry.rank}";
        nameText.text = entry.username;
        scoreText.text = FormatScore(entry.score);

        // Highlight player's own entry
        if (entry.isCurrentUser)
        {
            entryObj.GetComponent<Image>().color = new Color(1f, 1f, 0.5f, 0.3f);
        }

        // Medal icons for top 3
        if (entry.rank == 1)
        {
            rankText.text = "🥇";
        }
        else if (entry.rank == 2)
        {
            rankText.text = "🥈";
        }
        else if (entry.rank == 3)
        {
            rankText.text = "🥉";
        }
    }

    string FormatScore(float score)
    {
        return score.ToString("N0"); // 1,000,000
    }

    void ShowErrorMessage(string message)
    {
        Debug.LogWarning(message);
        // Show UI error popup
    }
}
```

---

## Cloud Save Manager

Auto-save system with periodic saves and manual save/load:

```csharp
using UnityEngine;
using SolisGames;
using System.Collections.Generic;

public class CloudSaveManager : MonoBehaviour
{
    [Header("Settings")]
    public float autoSaveInterval = 300f; // 5 minutes

    private const string SAVE_KEY = "player_progress";
    private float autoSaveTimer;

    [System.Serializable]
    public class PlayerSave
    {
        public int level;
        public int coins;
        public int experience;
        public List<string> unlockedLevels = new List<string>();
        public List<string> unlockedItems = new List<string>();
        public Dictionary<string, int> stats = new Dictionary<string, int>();
        public string lastPlayedDate;
    }

    private PlayerSave currentSave;

    void Start()
    {
        LoadGame();
    }

    void Update()
    {
        // Auto-save timer
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;
            SaveGame();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Save when game is paused/backgrounded
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    void OnApplicationQuit()
    {
        // Save when game is closed
        SaveGame();
    }

    public async void SaveGame()
    {
        try
        {
            // Update save data
            currentSave.lastPlayedDate = System.DateTime.UtcNow.ToString();

            bool success = await SolisSDK.CloudSave.SaveAsync(SAVE_KEY, currentSave);

            if (success)
            {
                Debug.Log("✅ Game saved to cloud");
                SolisSDK.Analytics.TrackEvent("game_saved");
            }
            else
            {
                Debug.LogWarning("⚠️ Cloud save failed (might be offline)");
                SaveLocally(); // Fallback to PlayerPrefs
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Save failed: {ex.Message}");
            SaveLocally();
        }
    }

    public async void LoadGame()
    {
        try
        {
            var loadedSave = await SolisSDK.CloudSave.LoadAsync<PlayerSave>(SAVE_KEY);

            if (loadedSave != null && loadedSave.level > 0)
            {
                currentSave = loadedSave;
                Debug.Log($"✅ Game loaded from cloud (Level {currentSave.level})");
            }
            else
            {
                // No cloud save found - try local or create new
                if (HasLocalSave())
                {
                    LoadLocally();
                }
                else
                {
                    CreateNewSave();
                }
            }

            ApplySaveData();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Load failed: {ex.Message}");
            LoadLocally();
        }
    }

    void CreateNewSave()
    {
        currentSave = new PlayerSave
        {
            level = 1,
            coins = 0,
            experience = 0,
            lastPlayedDate = System.DateTime.UtcNow.ToString()
        };

        Debug.Log("🆕 Created new save");
    }

    void ApplySaveData()
    {
        // Apply loaded data to game
        Debug.Log($"Level: {currentSave.level}");
        Debug.Log($"Coins: {currentSave.coins}");
        Debug.Log($"Experience: {currentSave.experience}");
        Debug.Log($"Unlocked Levels: {currentSave.unlockedLevels.Count}");

        SolisSDK.Analytics.TrackEvent("game_loaded", new Dictionary<string, object>
        {
            { "level", currentSave.level },
            { "coins", currentSave.coins }
        });
    }

    // Public methods to update save data
    public void AddCoins(int amount)
    {
        currentSave.coins += amount;
    }

    public void AddExperience(int amount)
    {
        currentSave.experience += amount;
    }

    public void UnlockLevel(string levelId)
    {
        if (!currentSave.unlockedLevels.Contains(levelId))
        {
            currentSave.unlockedLevels.Add(levelId);
        }
    }

    public void SetLevel(int level)
    {
        currentSave.level = level;
    }

    // Fallback: Local save using PlayerPrefs
    void SaveLocally()
    {
        string json = JsonUtility.ToJson(currentSave);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("💾 Saved locally (PlayerPrefs)");
    }

    void LoadLocally()
    {
        if (HasLocalSave())
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            currentSave = JsonUtility.FromJson<PlayerSave>(json);
            Debug.Log("💾 Loaded from local save");
        }
        else
        {
            CreateNewSave();
        }
    }

    bool HasLocalSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}
```

---

## Achievement Tracker

Track achievement progress and unlock with notifications:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;

public class AchievementTracker : MonoBehaviour
{
    [Header("UI References")]
    public GameObject achievementPopupPrefab;
    public Transform popupContainer;

    void Start()
    {
        // Example: Track various achievements
        TrackFirstWin();
        TrackCoinsCollected(150);
        TrackLevelCompleted(5);
    }

    public async void TrackFirstWin()
    {
        try
        {
            var achievement = await SolisSDK.Achievements.UnlockAsync("first_win");

            if (achievement != null)
            {
                ShowAchievementPopup(achievement);
                Debug.Log($"🏆 Unlocked: {achievement.name} (+{achievement.xp} XP)");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Achievement unlock failed: {ex.Message}");
        }
    }

    public async void TrackCoinsCollected(int totalCoins)
    {
        try
        {
            // Update progress toward "Coin Collector" achievement
            await SolisSDK.Achievements.ProgressAsync(
                "coin_collector",
                totalCoins,
                1000 // Target: collect 1000 coins
            );

            Debug.Log($"Progress: {totalCoins}/1000 coins");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Achievement progress failed: {ex.Message}");
        }
    }

    public async void TrackLevelCompleted(int level)
    {
        // Check if this unlocks "Speedrunner" achievement
        if (level >= 10)
        {
            var achievement = await SolisSDK.Achievements.UnlockAsync("speedrunner");

            if (achievement != null)
            {
                ShowAchievementPopup(achievement);
            }
        }

        // Track cumulative progress
        await SolisSDK.Achievements.ProgressAsync("complete_all_levels", level, 50);
    }

    public async void ShowAchievementStats()
    {
        try
        {
            var stats = await SolisSDK.Achievements.GetStatsAsync();

            Debug.Log($"Player Stats:");
            Debug.Log($"  Level: {stats.level}");
            Debug.Log($"  Total XP: {stats.totalXP}");
            Debug.Log($"  Total Achievements: {stats.totalUnlocked}/{stats.totalAvailable}");
            Debug.Log($"  Completion: {stats.completionPercentage:F1}%");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to get achievement stats: {ex.Message}");
        }
    }

    void ShowAchievementPopup(AchievementData achievement)
    {
        GameObject popup = Instantiate(achievementPopupPrefab, popupContainer);

        var iconImage = popup.transform.Find("Icon").GetComponent<Image>();
        var nameText = popup.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        var descText = popup.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        var xpText = popup.transform.Find("XP").GetComponent<TextMeshProUGUI>();

        nameText.text = achievement.name;
        descText.text = achievement.description;
        xpText.text = $"+{achievement.xp} XP";

        // Set rarity color
        Color rarityColor = achievement.rarity switch
        {
            "common" => Color.gray,
            "uncommon" => new Color(0.3f, 0.7f, 1f), // Blue
            "rare" => new Color(0.8f, 0.3f, 1f), // Purple
            _ => Color.white
        };

        popup.GetComponent<Image>().color = rarityColor;

        // Play sound effect
        AudioSource.PlayClipAtPoint(achievementSound, Camera.main.transform.position);

        // Auto-hide after 5 seconds
        Destroy(popup, 5f);
    }

    public AudioClip achievementSound; // Assign in Inspector
}
```

---

## Tournament UI

Display active tournaments and join:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;
using System.Collections.Generic;

public class TournamentUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform tournamentListContainer;
    public GameObject tournamentCardPrefab;
    public Button refreshButton;

    void Start()
    {
        refreshButton.onClick.AddListener(RefreshTournaments);
        RefreshTournaments();
    }

    async void RefreshTournaments()
    {
        try
        {
            refreshButton.interactable = false;

            // Get active tournaments
            var tournaments = await SolisSDK.Tournaments.ListAsync(status: "active");

            // Clear existing cards
            foreach (Transform child in tournamentListContainer)
            {
                Destroy(child.gameObject);
            }

            // Create card for each tournament
            foreach (var tournament in tournaments)
            {
                CreateTournamentCard(tournament);
            }

            Debug.Log($"Found {tournaments.Count} active tournaments");

            refreshButton.interactable = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load tournaments: {ex.Message}");
            refreshButton.interactable = true;
        }
    }

    void CreateTournamentCard(TournamentData tournament)
    {
        GameObject card = Instantiate(tournamentCardPrefab, tournamentListContainer);

        var nameText = card.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        var prizeText = card.transform.Find("Prize").GetComponent<TextMeshProUGUI>();
        var participantsText = card.transform.Find("Participants").GetComponent<TextMeshProUGUI>();
        var endTimeText = card.transform.Find("EndTime").GetComponent<TextMeshProUGUI>();
        var joinButton = card.transform.Find("JoinButton").GetComponent<Button>();

        nameText.text = tournament.name;
        prizeText.text = $"Prize: {tournament.prizePool} coins";
        participantsText.text = $"{tournament.participantCount} players";
        endTimeText.text = $"Ends: {FormatTimeRemaining(tournament.endTime)}";

        joinButton.onClick.AddListener(() => JoinTournament(tournament.id));
    }

    async void JoinTournament(string tournamentId)
    {
        try
        {
            bool success = await SolisSDK.Tournaments.JoinAsync(tournamentId);

            if (success)
            {
                Debug.Log("✅ Joined tournament!");
                ShowMessage("You've joined the tournament!");
                RefreshTournaments();
            }
            else
            {
                ShowMessage("Could not join tournament");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join tournament: {ex.Message}");
            ShowMessage("Failed to join tournament");
        }
    }

    string FormatTimeRemaining(string endTime)
    {
        // Parse and format end time
        if (System.DateTime.TryParse(endTime, out var endDateTime))
        {
            var timeRemaining = endDateTime - System.DateTime.UtcNow;

            if (timeRemaining.TotalHours >= 24)
            {
                return $"{(int)timeRemaining.TotalDays}d {timeRemaining.Hours}h";
            }
            else if (timeRemaining.TotalHours >= 1)
            {
                return $"{timeRemaining.Hours}h {timeRemaining.Minutes}m";
            }
            else
            {
                return $"{timeRemaining.Minutes}m";
            }
        }

        return "Soon";
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        // Show UI message
    }
}
```

---

## Rewarded Ads

Implement rewarded video ads with player choice:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;

public class RewardedAdManager : MonoBehaviour
{
    [Header("UI References")]
    public Button watchAdButton;
    public TextMeshProUGUI rewardText;

    [Header("Settings")]
    public int coinsPerAd = 100;

    void Start()
    {
        watchAdButton.onClick.AddListener(ShowRewardedAd);
        UpdateButtonText();
    }

    async void ShowRewardedAd()
    {
        try
        {
            watchAdButton.interactable = false;
            watchAdButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading Ad...";

            bool adWatched = await SolisSDK.Ads.ShowRewardedAsync();

            if (adWatched)
            {
                // Player watched the full ad - grant reward
                GrantReward();
                Debug.Log("✅ Ad watched, reward granted!");

                // Track ad view
                SolisSDK.Analytics.TrackEvent("rewarded_ad_watched");
            }
            else
            {
                // Player closed ad or ad failed to load
                Debug.Log("⚠️ Ad was not watched");
                ShowMessage("Ad unavailable right now. Try again later!");
            }

            watchAdButton.interactable = true;
            UpdateButtonText();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Rewarded ad failed: {ex.Message}");
            watchAdButton.interactable = true;
            UpdateButtonText();
        }
    }

    void GrantReward()
    {
        // Give player the reward
        PlayerData.coins += coinsPerAd;

        // Show reward UI
        ShowMessage($"You earned {coinsPerAd} coins!");

        // Play reward sound/animation
        PlayRewardAnimation();

        // Save progress
        FindObjectOfType<CloudSaveManager>()?.SaveGame();
    }

    void UpdateButtonText()
    {
        rewardText.text = $"Watch Ad for {coinsPerAd} Coins";
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        // Show UI popup
    }

    void PlayRewardAnimation()
    {
        // Play coin animation, particle effects, etc.
    }
}

// Example: Offer ad to continue after game over
public class GameOverScreen : MonoBehaviour
{
    public async void OnContinueWithAdButtonClicked()
    {
        bool adWatched = await SolisSDK.Ads.ShowRewardedAsync();

        if (adWatched)
        {
            // Grant continue
            GameManager.Instance.ContinueGame();
            gameObject.SetActive(false);
        }
        else
        {
            // Show alternative (pay with coins, return to menu, etc.)
            ShowMessage("Ad unavailable. Continue with 100 coins instead?");
        }
    }
}
```

---

## Friends List

Display friends with presence status:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;
using System.Collections.Generic;

public class FriendsListUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform friendsContainer;
    public GameObject friendEntryPrefab;
    public Button refreshButton;

    void Start()
    {
        refreshButton.onClick.AddListener(RefreshFriendsList);
        RefreshFriendsList();
    }

    async void RefreshFriendsList()
    {
        try
        {
            refreshButton.interactable = false;

            var friends = await SolisSDK.Friends.ListAsync();

            // Clear existing entries
            foreach (Transform child in friendsContainer)
            {
                Destroy(child.gameObject);
            }

            // Sort by status (online first)
            friends.Sort((a, b) => {
                int statusA = a.status == "online" ? 0 : 1;
                int statusB = b.status == "online" ? 0 : 1;
                return statusA.CompareTo(statusB);
            });

            // Create entry for each friend
            foreach (var friend in friends)
            {
                CreateFriendEntry(friend);
            }

            Debug.Log($"Friends list: {friends.Count} friends");

            refreshButton.interactable = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load friends: {ex.Message}");
            refreshButton.interactable = true;
        }
    }

    void CreateFriendEntry(FriendData friend)
    {
        GameObject entry = Instantiate(friendEntryPrefab, friendsContainer);

        var nameText = entry.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        var statusText = entry.transform.Find("Status").GetComponent<TextMeshProUGUI>();
        var statusIndicator = entry.transform.Find("StatusIndicator").GetComponent<Image>();

        nameText.text = friend.username;

        // Status indicator color
        Color statusColor = friend.status switch
        {
            "online" => Color.green,
            "away" => Color.yellow,
            "offline" => Color.gray,
            _ => Color.white
        };

        statusIndicator.color = statusColor;

        // Status text
        if (!string.IsNullOrEmpty(friend.currentGame))
        {
            statusText.text = $"Playing {friend.currentGame}";
        }
        else if (!string.IsNullOrEmpty(friend.customStatus))
        {
            statusText.text = friend.customStatus;
        }
        else
        {
            statusText.text = friend.status;
        }
    }
}
```

---

## In-Game Chat

Simple chat system:

```csharp
using UnityEngine;
using UnityEngine.UI;
using SolisGames;
using TMPro;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform chatContainer;
    public GameObject chatMessagePrefab;
    public TMP_InputField messageInput;
    public Button sendButton;
    public ScrollRect scrollRect;

    [Header("Settings")]
    public string channelId = "lobby_1";
    public int maxMessages = 50;

    private List<GameObject> messageObjects = new List<GameObject>();

    void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
        messageInput.onSubmit.AddListener((text) => SendMessage());

        LoadChatHistory();
    }

    async void LoadChatHistory()
    {
        try
        {
            var messages = await SolisSDK.Chat.GetMessagesAsync(channelId, limit: maxMessages);

            foreach (var message in messages)
            {
                DisplayMessage(message);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load chat: {ex.Message}");
        }
    }

    async void SendMessage()
    {
        string text = messageInput.text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        try
        {
            bool success = await SolisSDK.Chat.SendMessageAsync(channelId, text);

            if (success)
            {
                messageInput.text = "";
                Debug.Log("Message sent!");

                // Message will appear when we refresh
                LoadChatHistory();
            }
            else
            {
                ShowError("Failed to send message");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Send message failed: {ex.Message}");
            ShowError("Could not send message");
        }
    }

    void DisplayMessage(ChatMessage message)
    {
        GameObject msgObj = Instantiate(chatMessagePrefab, chatContainer);

        var usernameText = msgObj.transform.Find("Username").GetComponent<TextMeshProUGUI>();
        var messageText = msgObj.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        var timeText = msgObj.transform.Find("Time").GetComponent<TextMeshProUGUI>();

        usernameText.text = message.username;
        messageText.text = message.content;
        timeText.text = FormatTime(message.timestamp);

        // Highlight own messages
        if (message.isOwnMessage)
        {
            msgObj.GetComponent<Image>().color = new Color(0.5f, 0.7f, 1f, 0.2f);
        }

        // Show moderation flag
        if (message.flagged)
        {
            messageText.text = "[Message flagged by moderation]";
            messageText.color = Color.red;
        }

        messageObjects.Add(msgObj);

        // Limit message count
        if (messageObjects.Count > maxMessages)
        {
            Destroy(messageObjects[0]);
            messageObjects.RemoveAt(0);
        }

        // Scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    string FormatTime(string timestamp)
    {
        if (System.DateTime.TryParse(timestamp, out var time))
        {
            return time.ToString("HH:mm");
        }
        return "";
    }

    void ShowError(string error)
    {
        Debug.LogWarning(error);
    }
}
```

---

## Complete Game Manager

Full integration example:

```csharp
using UnityEngine;
using SolisGames;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public string apiKey = "your-api-key-here";

    private bool isSDKReady = false;
    private CloudSaveManager saveManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSDK();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void InitializeSDK()
    {
        Debug.Log("Initializing Solis Games SDK...");

        bool success = await SolisSDK.InitAsync(apiKey);

        if (success)
        {
            isSDKReady = true;
            Debug.Log("✅ SDK initialized successfully!");

            OnSDKReady();
        }
        else
        {
            Debug.LogError("❌ SDK initialization failed");
            ShowErrorScreen("Could not connect to game services");
        }
    }

    async void OnSDKReady()
    {
        // Get user data
        var user = await SolisSDK.User.GetUserAsync();
        Debug.Log($"Welcome back, {user.username}!");

        // Load player save
        saveManager = GetComponent<CloudSaveManager>();
        saveManager?.LoadGame();

        // Track session start
        SolisSDK.Analytics.TrackEvent("session_started");

        // Show main menu
        ShowMainMenu();
    }

    public async void OnLevelComplete(int level, float score, float time)
    {
        if (!isSDKReady) return;

        // Save progress
        saveManager?.SetLevel(level + 1);
        saveManager?.AddExperience(100);
        saveManager?.SaveGame();

        // Submit to leaderboard
        await SolisSDK.Leaderboards.SubmitAsync("high_scores", score, new Dictionary<string, object>
        {
            { "level", level },
            { "time", time }
        });

        // Track analytics
        SolisSDK.Analytics.TrackEvent("level_complete", new Dictionary<string, object>
        {
            { "level", level },
            { "score", score },
            { "time", time }
        });

        // Check achievements
        await CheckAchievements(level, score);

        Debug.Log($"Level {level} complete! Score: {score}");
    }

    async Task CheckAchievements(int level, float score)
    {
        // First win
        if (level == 1)
        {
            await SolisSDK.Achievements.UnlockAsync("first_win");
        }

        // Complete 10 levels
        if (level >= 10)
        {
            await SolisSDK.Achievements.UnlockAsync("veteran");
        }

        // High score
        if (score >= 10000)
        {
            await SolisSDK.Achievements.UnlockAsync("high_scorer");
        }
    }

    public async void OnGameOver(float finalScore)
    {
        if (!isSDKReady) return;

        // Track game over
        SolisSDK.Analytics.TrackEvent("game_over", new Dictionary<string, object>
        {
            { "score", finalScore }
        });

        // Show interstitial ad (every 3rd game over)
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed + 1);

        if (gamesPlayed % 3 == 0)
        {
            await SolisSDK.Ads.ShowInterstitialAsync();
        }
    }

    void ShowMainMenu()
    {
        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void ShowErrorScreen(string error)
    {
        Debug.LogError(error);
        // Show error UI
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isSDKReady)
        {
            // Track session pause
            SolisSDK.Analytics.TrackEvent("session_paused");

            // Save game
            saveManager?.SaveGame();
        }
    }

    void OnApplicationQuit()
    {
        if (isSDKReady)
        {
            // Track session end
            SolisSDK.Analytics.TrackEvent("session_ended");

            // Final save
            saveManager?.SaveGame();
        }
    }
}
```

---

## More Examples

For complete, runnable example projects:

- **QuickStart Sample:** `Samples~/QuickStart/` - Minimal integration
- **FullFeatures Sample:** `Samples~/FullFeatures/` - All SDK features
- **2DPlatformer Sample:** `Samples~/2DPlatformer/` - Complete game example

---

## Need Help?

- **API Reference:** [API-Reference.md](API-Reference.md)
- **Troubleshooting:** [Troubleshooting.md](Troubleshooting.md)
- **Discord:** https://discord.gg/solisgames
- **Support:** support@solisgames.com
