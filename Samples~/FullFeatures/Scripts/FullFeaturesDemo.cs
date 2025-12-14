using UnityEngine;
using SolisGames;
using System.Collections.Generic;

/// <summary>
/// Full Features Demo - Comprehensive showcase of all SDK features
/// This example demonstrates every module of the Solis Games SDK
/// </summary>
public class FullFeaturesDemo : MonoBehaviour
{
    [Header("Configuration")]
    public string apiKey = "your-api-key-here";

    [Header("Feature Toggles")]
    public bool demoUser = true;
    public bool demoLeaderboards = true;
    public bool demoAchievements = true;
    public bool demoTournaments = true;
    public bool demoCloudSave = true;
    public bool demoAnalytics = true;
    public bool demoAds = true;
    public bool demoFriends = true;
    public bool demoChat = true;

    async void Start()
    {
        Debug.Log("========================================");
        Debug.Log("  SOLIS GAMES SDK - FULL FEATURES DEMO");
        Debug.Log("========================================\n");

        // Initialize SDK
        bool success = await SolisSDK.InitAsync(apiKey);

        if (!success)
        {
            Debug.LogError("SDK initialization failed!");
            return;
        }

        Debug.Log("✅ SDK initialized!\n");

        // Run all feature demos
        if (demoUser) await DemoUserModule();
        if (demoLeaderboards) await DemoLeaderboards();
        if (demoAchievements) await DemoAchievements();
        if (demoTournaments) await DemoTournaments();
        if (demoCloudSave) await DemoCloudSave();
        if (demoAnalytics) DemoAnalytics();
        if (demoAds) await DemoAds();
        if (demoFriends) await DemoFriends();
        if (demoChat) await DemoChat();

        Debug.Log("\n========================================");
        Debug.Log("  DEMO COMPLETE!");
        Debug.Log("========================================");
    }

    /// <summary>
    /// Demo: User Management
    /// </summary>
    async Task DemoUserModule()
    {
        Debug.Log("--- USER MODULE ---");

        try
        {
            var user = await SolisSDK.User.GetUserAsync();

            Debug.Log($"User ID: {user.id}");
            Debug.Log($"Username: {user.username}");
            Debug.Log($"Email: {user.email}");
            Debug.Log($"Premium: {user.isPremium}");
            Debug.Log($"Created: {user.createdAt}\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"User demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Leaderboards & Rankings
    /// </summary>
    async Task DemoLeaderboards()
    {
        Debug.Log("--- LEADERBOARDS MODULE ---");

        try
        {
            // Submit a score
            float testScore = Random.Range(1000, 9999);
            var submitResult = await SolisSDK.Leaderboards.SubmitAsync(
                "high_scores",
                testScore,
                new Dictionary<string, object>
                {
                    { "level", 5 },
                    { "time", 120.5f }
                }
            );

            Debug.Log($"Score submitted: {submitResult.score}");
            Debug.Log($"Your rank: #{submitResult.rank}");

            if (submitResult.flaggedForReview)
            {
                Debug.LogWarning($"Score flagged: {string.Join(", ", submitResult.flags)}");
            }

            // Get top 10 rankings
            var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 10);

            Debug.Log($"\nTop {rankings.entries.Count} Players:");
            foreach (var entry in rankings.entries)
            {
                string marker = entry.isCurrentUser ? " <-- YOU" : "";
                Debug.Log($"  #{entry.rank} - {entry.username}: {entry.score:N0}{marker}");
            }

            // Get nearby ranks
            var nearby = await SolisSDK.Leaderboards.GetNearbyAsync("high_scores", range: 2);

            Debug.Log($"\nYour nearby ranks:");
            foreach (var entry in nearby.entries)
            {
                string marker = entry.isCurrentUser ? " <-- YOU" : "";
                Debug.Log($"  #{entry.rank} - {entry.username}: {entry.score:N0}{marker}");
            }

            Debug.Log("");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Leaderboards demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Achievements & Progression
    /// </summary>
    async Task DemoAchievements()
    {
        Debug.Log("--- ACHIEVEMENTS MODULE ---");

        try
        {
            // Get player stats
            var stats = await SolisSDK.Achievements.GetStatsAsync();

            Debug.Log($"Level: {stats.level}");
            Debug.Log($"Total XP: {stats.totalXP:N0}");
            Debug.Log($"Achievements: {stats.totalUnlocked}/{stats.totalAvailable}");
            Debug.Log($"Completion: {stats.completionPercentage:F1}%");

            // Unlock an achievement
            var achievement = await SolisSDK.Achievements.UnlockAsync("first_win");

            if (achievement != null)
            {
                Debug.Log($"\n🏆 Achievement Unlocked: {achievement.name}");
                Debug.Log($"   Description: {achievement.description}");
                Debug.Log($"   XP Earned: +{achievement.xp}");
                Debug.Log($"   Rarity: {achievement.rarity}");
            }

            // Track progress toward an achievement
            await SolisSDK.Achievements.ProgressAsync("coin_collector", 150, 1000);
            Debug.Log("Progress tracked: 150/1000 coins collected\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Achievements demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Tournaments & Competitive Play
    /// </summary>
    async Task DemoTournaments()
    {
        Debug.Log("--- TOURNAMENTS MODULE ---");

        try
        {
            // List active tournaments
            var tournaments = await SolisSDK.Tournaments.ListAsync(status: "active");

            Debug.Log($"Active tournaments: {tournaments.Count}");

            foreach (var tournament in tournaments)
            {
                Debug.Log($"\n🏆 {tournament.name}");
                Debug.Log($"   Participants: {tournament.participantCount}");
                Debug.Log($"   Prize Pool: {tournament.prizePool} coins");
                Debug.Log($"   Status: {tournament.status}");
                Debug.Log($"   Ends: {tournament.endTime}");
            }

            // Join a tournament (if any available)
            if (tournaments.Count > 0)
            {
                bool joined = await SolisSDK.Tournaments.JoinAsync(tournaments[0].id);
                Debug.Log($"\nJoined tournament: {joined}");
            }

            Debug.Log("");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Tournaments demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Cloud Saves
    /// </summary>
    async Task DemoCloudSave()
    {
        Debug.Log("--- CLOUD SAVE MODULE ---");

        try
        {
            // Define save data
            var saveData = new PlayerSaveData
            {
                level = 10,
                coins = 5000,
                experience = 25000,
                unlockedLevels = new List<string> { "level_1", "level_2", "level_3" },
                lastPlayedDate = System.DateTime.UtcNow.ToString()
            };

            // Save to cloud
            bool saved = await SolisSDK.CloudSave.SaveAsync("player_progress", saveData);
            Debug.Log($"Save successful: {saved}");

            // Load from cloud
            var loadedSave = await SolisSDK.CloudSave.LoadAsync<PlayerSaveData>("player_progress");

            Debug.Log($"\nLoaded save data:");
            Debug.Log($"  Level: {loadedSave.level}");
            Debug.Log($"  Coins: {loadedSave.coins:N0}");
            Debug.Log($"  Experience: {loadedSave.experience:N0}");
            Debug.Log($"  Unlocked Levels: {loadedSave.unlockedLevels.Count}");
            Debug.Log($"  Last Played: {loadedSave.lastPlayedDate}\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Cloud Save demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Analytics & Event Tracking
    /// </summary>
    void DemoAnalytics()
    {
        Debug.Log("--- ANALYTICS MODULE ---");

        // Track simple event
        SolisSDK.Analytics.TrackEvent("demo_started");
        Debug.Log("Event tracked: demo_started");

        // Track event with custom data
        SolisSDK.Analytics.TrackEvent("level_complete", new Dictionary<string, object>
        {
            { "level", 5 },
            { "score", 1337 },
            { "time", 120.5f },
            { "deaths", 3 },
            { "collectibles", 15 }
        });
        Debug.Log("Event tracked: level_complete (with custom data)");

        // Track player progression
        SolisSDK.Analytics.TrackEvent("player_progression", new Dictionary<string, object>
        {
            { "level", 10 },
            { "coins", 5000 },
            { "playtime_minutes", 180 }
        });
        Debug.Log("Event tracked: player_progression\n");
    }

    /// <summary>
    /// Demo: Ads (Rewarded & Interstitial)
    /// </summary>
    async Task DemoAds()
    {
        Debug.Log("--- ADS MODULE ---");

        try
        {
            // Show rewarded ad
            Debug.Log("Showing rewarded ad...");
            bool rewardedWatched = await SolisSDK.Ads.ShowRewardedAsync();

            if (rewardedWatched)
            {
                Debug.Log("✅ Rewarded ad watched - Grant reward to player!");
            }
            else
            {
                Debug.Log("⚠️ Rewarded ad skipped or unavailable");
            }

            // Show interstitial ad
            Debug.Log("\nShowing interstitial ad...");
            bool interstitialShown = await SolisSDK.Ads.ShowInterstitialAsync();

            if (interstitialShown)
            {
                Debug.Log("✅ Interstitial ad shown");
            }
            else
            {
                Debug.Log("⚠️ Interstitial ad unavailable");
            }

            Debug.Log("");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ads demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: Friends & Social
    /// </summary>
    async Task DemoFriends()
    {
        Debug.Log("--- FRIENDS MODULE ---");

        try
        {
            // Get friends list
            var friends = await SolisSDK.Friends.ListAsync();

            Debug.Log($"Total friends: {friends.Count}");

            foreach (var friend in friends)
            {
                string statusIcon = friend.status switch
                {
                    "online" => "🟢",
                    "away" => "🟡",
                    "offline" => "⚫",
                    _ => "⚪"
                };

                Debug.Log($"{statusIcon} {friend.username} - {friend.status}");

                if (!string.IsNullOrEmpty(friend.currentGame))
                {
                    Debug.Log($"   Playing: {friend.currentGame}");
                }
            }

            // Get friend requests
            var requests = await SolisSDK.Friends.GetRequestsAsync();
            Debug.Log($"\nPending friend requests: {requests.Count}\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Friends demo failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demo: In-Game Chat
    /// </summary>
    async Task DemoChat()
    {
        Debug.Log("--- CHAT MODULE ---");

        try
        {
            string channelId = "demo_lobby";

            // Send a message
            bool sent = await SolisSDK.Chat.SendMessageAsync(channelId, "Hello from Unity SDK!");
            Debug.Log($"Message sent: {sent}");

            // Get recent messages
            var messages = await SolisSDK.Chat.GetMessagesAsync(channelId, limit: 10);

            Debug.Log($"\nRecent messages ({messages.Count}):");
            foreach (var msg in messages)
            {
                string flaggedMarker = msg.flagged ? " [FLAGGED]" : "";
                Debug.Log($"  [{msg.timestamp}] {msg.username}: {msg.content}{flaggedMarker}");
            }

            Debug.Log("");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Chat demo failed: {ex.Message}\n");
        }
    }
}

/// <summary>
/// Example save data structure for Cloud Save demo
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public int level;
    public int coins;
    public int experience;
    public List<string> unlockedLevels = new List<string>();
    public string lastPlayedDate;
}
