using UnityEngine;
using SolisGames;

/// <summary>
/// Quick Start Example - Minimal integration of Solis Games SDK
/// This example shows the simplest way to get started with the SDK
/// </summary>
public class QuickStartExample : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Get your API key from the Studio Dashboard at solisgames.com/studio")]
    public string apiKey = "your-api-key-here";

    async void Start()
    {
        Debug.Log("=== Solis Games SDK - Quick Start Example ===");

        // Step 1: Initialize the SDK
        Debug.Log("Initializing SDK...");
        bool success = await SolisSDK.InitAsync(apiKey);

        if (success)
        {
            Debug.Log("✅ SDK initialized successfully!");

            // Step 2: Get user data
            await GetUserExample();

            // Step 3: Submit a score to leaderboard
            await SubmitScoreExample();

            // Step 4: Track an analytics event
            TrackEventExample();

            // Step 5: Save game data to cloud
            await SaveGameExample();

            Debug.Log("=== Quick Start Example Complete! ===");
        }
        else
        {
            Debug.LogError("❌ SDK initialization failed. Check your API key.");
        }
    }

    /// <summary>
    /// Get authenticated user data
    /// </summary>
    async Task GetUserExample()
    {
        try
        {
            var user = await SolisSDK.User.GetUserAsync();

            Debug.Log($"User Data:");
            Debug.Log($"  Username: {user.username}");
            Debug.Log($"  Email: {user.email}");
            Debug.Log($"  Premium: {user.isPremium}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Get user failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Submit a score to a leaderboard
    /// </summary>
    async Task SubmitScoreExample()
    {
        try
        {
            float score = Random.Range(100, 1000);

            var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);

            if (result.success)
            {
                Debug.Log($"Score Submitted:");
                Debug.Log($"  Score: {result.score}");
                Debug.Log($"  Rank: #{result.rank}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Submit score failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Track a custom analytics event
    /// </summary>
    void TrackEventExample()
    {
        SolisSDK.Analytics.TrackEvent("quick_start_complete", new System.Collections.Generic.Dictionary<string, object>
        {
            { "timestamp", System.DateTime.UtcNow.ToString() },
            { "unity_version", Application.unityVersion }
        });

        Debug.Log("Analytics event tracked: quick_start_complete");
    }

    /// <summary>
    /// Save game data to cloud
    /// </summary>
    async Task SaveGameExample()
    {
        try
        {
            // Define your save data structure
            var saveData = new
            {
                level = 1,
                coins = 100,
                timestamp = System.DateTime.UtcNow.ToString()
            };

            bool success = await SolisSDK.CloudSave.SaveAsync("player_progress", saveData);

            if (success)
            {
                Debug.Log("Game data saved to cloud!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Cloud save failed: {ex.Message}");
        }
    }
}
