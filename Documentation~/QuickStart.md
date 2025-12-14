# Quick Start Guide

Get started with the Solis Games Unity SDK in just 5 minutes!

## Prerequisites

✅ Unity 2022.3 LTS or newer
✅ Solis Games Unity SDK installed ([Installation Guide](Installation.md))
✅ API key from [Studio Dashboard](https://solisgames.com/studio)

---

## Step 1: Configure the SDK (1 minute)

1. Open Unity
2. Go to `Window > Solis Games > SDK Settings`
3. Enter your **API key** from the Studio Dashboard
4. (Optional) Enter your **Game ID** if you have one
5. Leave all features enabled (or customize as needed)
6. Click **Save Settings**

✅ **Done!** The SDK is now configured.

---

## Step 2: Initialize the SDK (2 minutes)

Create a new C# script called `GameInitializer.cs`:

```csharp
using UnityEngine;
using SolisGames;

public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        // Initialize the SDK
        bool success = await SolisSDK.InitAsync("your-api-key");

        if (success)
        {
            Debug.Log("Solis Games SDK initialized successfully!");

            // Get user information
            var user = await SolisSDK.User.GetAsync();
            Debug.Log($"Welcome, {user.username}!");
        }
        else
        {
            Debug.LogError("Failed to initialize Solis Games SDK");
        }
    }
}
```

**Important:** Replace `"your-api-key"` with your actual API key, or use the configured key:

```csharp
// The SDK will use the key from SDK Settings if you don't pass one
bool success = await SolisSDK.InitAsync(
    UnityEditor.EditorPrefs.GetString("SolisGames_ApiKey")
);
```

**Attach the script:**
1. Create an empty GameObject in your scene (right-click in Hierarchy > Create Empty)
2. Rename it to "GameManager"
3. Drag the `GameInitializer.cs` script onto it

---

## Step 3: Add Your First Feature (2 minutes)

Let's add a simple leaderboard! Update your `GameInitializer.cs`:

```csharp
using UnityEngine;
using SolisGames;
using System.Threading.Tasks;

public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        // Initialize SDK
        bool success = await SolisSDK.InitAsync("your-api-key");

        if (!success)
        {
            Debug.LogError("SDK initialization failed");
            return;
        }

        Debug.Log("SDK initialized! Testing features...");

        // Submit a score
        await SubmitScore(1000);

        // Get top 10 rankings
        await GetLeaderboard();
    }

    async Task SubmitScore(float score)
    {
        var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);

        if (result != null)
        {
            Debug.Log($"Score submitted! Your rank: #{result.rank}");
        }
    }

    async Task GetLeaderboard()
    {
        var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 10);

        if (rankings != null && rankings.entries != null)
        {
            Debug.Log($"Top {rankings.entries.Count} players:");

            foreach (var entry in rankings.entries)
            {
                Debug.Log($"#{entry.rank} - {entry.username}: {entry.score}");
            }
        }
    }
}
```

---

## Step 4: Test in Unity Editor

1. Press **Play** in Unity Editor
2. Check the Console for log messages:
   ```
   Solis Games SDK initialized successfully!
   Score submitted! Your rank: #42
   Top 10 players:
   #1 - Player1: 5000
   #2 - Player2: 4500
   ...
   ```

**Note:** In Unity Editor, the SDK uses mock data for testing. You'll see demo players and scores.

---

## Step 5: Build for WebGL

1. Go to `Window > Solis Games > Build & Deploy`
2. Set your **Output Path** (default: `Builds/WebGL`)
3. Choose **Compression** (Gzip recommended)
4. Click **🔨 Build WebGL**
5. Wait for the build to complete

✅ **The SDK script is automatically injected!** No manual setup needed.

---

## Step 6: Test Your WebGL Build

### Option A: Auto-run (Recommended for testing)
1. In the Build & Deploy window, enable **"Auto-run after build"**
2. Build again
3. Your default browser will open with the game

### Option B: Manual testing
1. Navigate to your build folder (e.g., `Builds/WebGL`)
2. Open `index.html` in your browser
3. Check the browser console for SDK initialization logs

---

## What's Next?

🎉 **Congratulations!** You've successfully integrated the Solis Games SDK!

### Explore More Features:

**Monetization:**
```csharp
// Show rewarded ad
bool rewarded = await SolisSDK.Ads.ShowRewardedAsync("reward_coins");
if (rewarded) {
    GiveCoins(100);
}
```

**Achievements:**
```csharp
// Unlock achievement
await SolisSDK.Achievements.UnlockAsync("first_win");
```

**Cloud Saves:**
```csharp
// Save game data
await SolisSDK.CloudSave.SaveAsync("player_progress", myPlayerData);

// Load game data
var data = await SolisSDK.CloudSave.LoadAsync<PlayerSaveData>("player_progress");
```

**Friends & Social:**
```csharp
// Get online friends
var friends = await SolisSDK.Friends.GetOnlineAsync();
```

**Chat:**
```csharp
// Send chat message
await SolisSDK.Chat.SendAsync("lobby_1", "GG everyone!");
```

---

## Full Example: Complete Game Manager

Here's a more complete example showing multiple features:

```csharp
using UnityEngine;
using SolisGames;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    private bool sdkReady = false;

    async void Start()
    {
        await InitializeSDK();
    }

    async Task InitializeSDK()
    {
        bool success = await SolisSDK.InitAsync("your-api-key");

        if (success)
        {
            Debug.Log("✅ SDK Ready!");
            sdkReady = true;

            // Get user info
            var user = await SolisSDK.User.GetAsync();
            Debug.Log($"Logged in as: {user.username}");

            // Load saved game
            await LoadGame();
        }
        else
        {
            Debug.LogError("❌ SDK initialization failed");
        }
    }

    async Task LoadGame()
    {
        var saveData = await SolisSDK.CloudSave.LoadAsync<PlayerSaveData>("save_slot_1");

        if (saveData != null)
        {
            Debug.Log($"Game loaded! Level: {saveData.level}, Coins: {saveData.coins}");
        }
        else
        {
            Debug.Log("No save data found, starting new game");
        }
    }

    public async void OnLevelComplete(int level, float score)
    {
        if (!sdkReady) return;

        // Track analytics
        await SolisSDK.Analytics.TrackEventAsync("level_complete", new System.Collections.Generic.Dictionary<string, object>
        {
            { "level", level },
            { "score", score }
        });

        // Submit to leaderboard
        var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
        Debug.Log($"Score submitted! Rank: #{result.rank}");

        // Check for achievement
        if (level == 10)
        {
            await SolisSDK.Achievements.UnlockAsync("complete_level_10");
        }
    }

    public async void OnRewardButtonClick()
    {
        bool rewarded = await SolisSDK.Ads.ShowRewardedAsync("reward_coins");

        if (rewarded)
        {
            Debug.Log("Player watched ad, giving reward!");
            // Give coins or other reward
        }
    }

    public async void SaveGame(PlayerSaveData data)
    {
        bool success = await SolisSDK.CloudSave.SaveAsync("save_slot_1", data);

        if (success)
        {
            Debug.Log("✅ Game saved!");
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public int level;
    public int coins;
    public string[] unlockedLevels;
}
```

---

## Need Help?

- 📖 [API Reference](API-Reference.md) - Complete documentation for all modules
- 💡 [Examples](Examples.md) - More code examples
- 🔧 [Troubleshooting](Troubleshooting.md) - Common issues and solutions
- 💬 [Get Support](https://github.com/solis-interactive/unity-sdk/issues)

---

## Common Next Steps

1. **Add UI for leaderboards** - Display rankings in your game
2. **Implement save/load** - Use Cloud Save for cross-device progress
3. **Add achievements** - Unlock achievements for player milestones
4. **Monetize with ads** - Show rewarded ads for coins/lives
5. **Add social features** - Show friends playing the game

Continue to the [API Reference](API-Reference.md) for complete documentation of all features!
