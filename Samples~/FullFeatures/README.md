# Full Features Sample

Comprehensive demonstration of every Solis Games SDK feature.

## What This Example Shows

✅ **User Management** - Get authenticated user data
✅ **Leaderboards** - Submit scores, fetch rankings, nearby ranks
✅ **Achievements** - Unlock achievements, track progress, view stats
✅ **Tournaments** - List tournaments, join competitions
✅ **Cloud Saves** - Save/load game data with type safety
✅ **Analytics** - Track custom events with metadata
✅ **Ads** - Rewarded videos, interstitial ads
✅ **Friends & Social** - Friends list, presence status
✅ **In-Game Chat** - Send messages, view chat history

## Setup Instructions

1. **Import this sample:**
   - Window > Package Manager > Solis Games SDK > Samples > Full Features > Import

2. **Configure your API key:**
   - Open the FullFeatures scene
   - Select the "FullFeaturesDemo" GameObject
   - Enter your API key in the Inspector

3. **Choose which features to demo:**
   - Toggle individual features on/off in the Inspector
   - Useful for testing specific modules

4. **Run the example:**
   - Press Play in Unity Editor
   - Watch the Console for detailed output

## Expected Console Output

```
========================================
  SOLIS GAMES SDK - FULL FEATURES DEMO
========================================

✅ SDK initialized!

--- USER MODULE ---
User ID: abc123
Username: DemoPlayer
Email: demo@example.com
Premium: False
Created: 2025-01-15T10:30:00Z

--- LEADERBOARDS MODULE ---
Score submitted: 7542
Your rank: #42

Top 10 Players:
  #1 - ProGamer: 9,999
  #2 - SpeedRunner: 8,750
  #3 - HighScore: 8,120
  ...
  #42 - DemoPlayer: 7,542 <-- YOU

Your nearby ranks:
  #40 - Player40: 7,650
  #41 - Player41: 7,580
  #42 - DemoPlayer: 7,542 <-- YOU
  #43 - Player43: 7,500
  #44 - Player44: 7,450

--- ACHIEVEMENTS MODULE ---
Level: 15
Total XP: 25,000
Achievements: 12/50
Completion: 24.0%

🏆 Achievement Unlocked: First Victory
   Description: Win your first game
   XP Earned: +100
   Rarity: common
Progress tracked: 150/1000 coins collected

--- TOURNAMENTS MODULE ---
Active tournaments: 2

🏆 Weekend Blitz
   Participants: 1,234
   Prize Pool: 10000 coins
   Status: active
   Ends: 2025-01-20T23:59:59Z

Joined tournament: True

--- CLOUD SAVE MODULE ---
Save successful: True

Loaded save data:
  Level: 10
  Coins: 5,000
  Experience: 25,000
  Unlocked Levels: 3
  Last Played: 2025-01-15T14:30:00Z

--- ANALYTICS MODULE ---
Event tracked: demo_started
Event tracked: level_complete (with custom data)
Event tracked: player_progression

--- ADS MODULE ---
Showing rewarded ad...
✅ Rewarded ad watched - Grant reward to player!

Showing interstitial ad...
✅ Interstitial ad shown

--- FRIENDS MODULE ---
Total friends: 5
🟢 Friend1 - online
   Playing: Space Raiders
🟡 Friend2 - away
⚫ Friend3 - offline

Pending friend requests: 2

--- CHAT MODULE ---
Message sent: True

Recent messages (3):
  [14:30:15] Player1: Hello!
  [14:30:20] Player2: GG everyone
  [14:30:25] DemoPlayer: Hello from Unity SDK!

========================================
  DEMO COMPLETE!
========================================
```

## Code Structure

### FullFeaturesDemo.cs
Main demo script that showcases all SDK modules. Each feature has its own demo method:

- `DemoUserModule()` - User authentication and profile
- `DemoLeaderboards()` - Competitive rankings
- `DemoAchievements()` - Progression system
- `DemoTournaments()` - Tournament system
- `DemoCloudSave()` - Cloud storage with type safety
- `DemoAnalytics()` - Event tracking
- `DemoAds()` - Monetization
- `DemoFriends()` - Social features
- `DemoChat()` - Messaging system

### PlayerSaveData.cs
Example save data structure showing how to use Cloud Save with custom types.

## Customization

### Toggle Features
Use the Inspector toggles to enable/disable specific demos:

```csharp
[Header("Feature Toggles")]
public bool demoUser = true;
public bool demoLeaderboards = true;
public bool demoAchievements = true;
// ... etc
```

### Modify Demo Data
Change test data to match your game:

```csharp
// In DemoLeaderboards()
float testScore = Random.Range(1000, 9999); // Change score range

// In DemoCloudSave()
var saveData = new PlayerSaveData
{
    level = 10,        // Change default values
    coins = 5000,
    // ...
};
```

## Common Use Cases

### Testing Leaderboards
1. Enable only `demoLeaderboards`
2. Run multiple times to see rank changes
3. Check Studio Dashboard for submitted scores

### Testing Cloud Saves
1. Enable only `demoCloudSave`
2. Modify `PlayerSaveData` structure
3. Run to test save/load with your data model

### Testing Achievements
1. Enable only `demoAchievements`
2. Create achievements in Studio Dashboard
3. Run to test unlock flow

## Integration Tips

### Error Handling
All demos include try/catch blocks:

```csharp
try
{
    var user = await SolisSDK.User.GetUserAsync();
    // ... use data
}
catch (System.Exception ex)
{
    Debug.LogError($"Failed: {ex.Message}");
    // Handle error gracefully
}
```

### Async/Await Pattern
Most SDK calls are async:

```csharp
async void Start()
{
    await SolisSDK.InitAsync(apiKey);
    await RunDemos();
}
```

### Type-Safe Cloud Saves
Use `[System.Serializable]` classes:

```csharp
[System.Serializable]
public class MyGameData
{
    public int level;
    public List<string> items;
}

await SolisSDK.CloudSave.SaveAsync<MyGameData>("key", data);
```

## Next Steps

1. **Adapt for Your Game:**
   - Copy relevant demo methods into your game code
   - Modify data structures to match your game

2. **Build & Test:**
   - Build for WebGL
   - Test on your domain (must be whitelisted in Studio)

3. **Study the Code:**
   - Each demo method is self-contained
   - Copy-paste examples into your own scripts

4. **Read Full Documentation:**
   - [API Reference](../../Documentation~/API-Reference.md)
   - [Examples](../../Documentation~/Examples.md)
   - [Troubleshooting](../../Documentation~/Troubleshooting.md)

## Files

- `Scripts/FullFeaturesDemo.cs` - Main demo script
- `README.md` - This file

## Need Help?

- **Quick Start:** Check the QuickStart sample for simpler examples
- **Documentation:** https://solisgames.com/docs/unity
- **Discord:** https://discord.gg/TZdBFBhW
- **Support:** support@solisgames.com
