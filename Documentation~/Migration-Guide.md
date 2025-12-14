# Migration Guide: JavaScript SDK → Unity Plugin

This guide helps you migrate from the JavaScript SDK to the native Unity C# plugin.

## Why Migrate?

The Unity Plugin offers several advantages over the JavaScript bridge:

| Feature | JavaScript SDK | Unity Plugin |
|---------|----------------|--------------|
| **Type Safety** | ❌ Runtime errors | ✅ Compile-time checking |
| **IntelliSense** | ❌ No autocomplete | ✅ Full autocomplete |
| **Async/Await** | ❌ Callbacks only | ✅ Modern async/await |
| **Editor Testing** | ❌ WebGL build required | ✅ Mock data in editor |
| **Code Navigation** | ❌ Jump to definition doesn't work | ✅ Full IDE integration |
| **Error Messages** | ❌ Generic JS errors | ✅ Clear C# exceptions |

---

## Migration Steps

### 1. Installation

**Before (JavaScript SDK):**
```html
<!-- Manually inject script in index.html -->
<script src="https://solisgames.com/solis-games-sdk-v1.js"
        data-api-key="your-api-key"
        data-game-id="your-game-id"></script>
```

**After (Unity Plugin):**
```
1. Window > Solis Games > SDK Settings
2. Enter API key
3. Build WebGL (SDK automatically injected)
```

---

### 2. SDK Initialization

**Before (JavaScript SDK):**
```csharp
// Use Application.ExternalEval or jslib plugin
Application.ExternalEval(@"
    window.SolisGames.SDK.init('your-api-key').then(() => {
        console.log('SDK initialized');
    });
");
```

**After (Unity Plugin):**
```csharp
using SolisGames;

public class GameManager : MonoBehaviour
{
    async void Start()
    {
        bool success = await SolisSDK.InitAsync("your-api-key");

        if (success)
        {
            Debug.Log("SDK initialized!");
        }
        else
        {
            Debug.LogError("SDK initialization failed");
        }
    }
}
```

---

### 3. User Authentication

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval(@"
    window.SolisGames.SDK.user.getUser().then((user) => {
        // Send data back to Unity via SendMessage
        gameObject.SendMessage('OnUserReceived', JSON.stringify(user));
    });
");
```

**After (Unity Plugin):**
```csharp
UserData user = await SolisSDK.User.GetUserAsync();

Debug.Log($"User: {user.username} (ID: {user.id})");
Debug.Log($"Email: {user.email}");
Debug.Log($"Premium: {user.isPremium}");
```

---

### 4. Analytics Events

**Before (JavaScript SDK):**
```csharp
string eventName = "level_complete";
string eventData = JsonUtility.ToJson(new { level = 5, score = 1000 });

Application.ExternalEval($@"
    window.SolisGames.SDK.analytics.track('{eventName}', {eventData});
");
```

**After (Unity Plugin):**
```csharp
SolisSDK.Analytics.TrackEvent("level_complete", new Dictionary<string, object>
{
    { "level", 5 },
    { "score", 1000 },
    { "time", 120.5f }
});
```

---

### 5. Cloud Saves

**Before (JavaScript SDK):**
```csharp
// Save
string saveData = JsonUtility.ToJson(playerData);
Application.ExternalEval($@"
    window.SolisGames.SDK.cloudSave.save('player_progress', {saveData})
        .then(() => console.log('Saved'));
");

// Load
Application.ExternalEval(@"
    window.SolisGames.SDK.cloudSave.load('player_progress')
        .then((data) => {
            gameObject.SendMessage('OnSaveLoaded', JSON.stringify(data));
        });
");
```

**After (Unity Plugin):**
```csharp
[System.Serializable]
public class PlayerSave
{
    public int level;
    public int coins;
    public List<string> unlockedLevels;
}

// Save
var save = new PlayerSave { level = 10, coins = 500 };
bool success = await SolisSDK.CloudSave.SaveAsync("player_progress", save);

// Load
PlayerSave loadedSave = await SolisSDK.CloudSave.LoadAsync<PlayerSave>("player_progress");
Debug.Log($"Loaded: Level {loadedSave.level}, Coins {loadedSave.coins}");
```

---

### 6. Leaderboards

**Before (JavaScript SDK):**
```csharp
// Submit score
Application.ExternalEval($@"
    window.SolisGames.SDK.leaderboards.submit('high_scores', {{ score: 1000 }})
        .then((result) => {{
            gameObject.SendMessage('OnScoreSubmitted', JSON.stringify(result));
        }});
");

// Get rankings
Application.ExternalEval(@"
    window.SolisGames.SDK.leaderboards.get('high_scores', { limit: 10 })
        .then((rankings) => {
            gameObject.SendMessage('OnRankingsReceived', JSON.stringify(rankings));
        });
");
```

**After (Unity Plugin):**
```csharp
// Submit score
var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", 1000);
Debug.Log($"Your rank: #{result.rank}");

// Get rankings
var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 10);

foreach (var entry in rankings.entries)
{
    Debug.Log($"#{entry.rank} - {entry.username}: {entry.score}");
}
```

---

### 7. Achievements

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval(@"
    window.SolisGames.SDK.achievements.unlock('first_win')
        .then((achievement) => {
            console.log('Achievement unlocked!');
        });
");
```

**After (Unity Plugin):**
```csharp
var achievement = await SolisSDK.Achievements.UnlockAsync("first_win");

if (achievement != null)
{
    Debug.Log($"🏆 Unlocked: {achievement.name} (+{achievement.xp} XP)");
    Debug.Log($"Rarity: {achievement.rarity}");
}
```

---

### 8. Ads (Rewarded & Interstitial)

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval(@"
    window.SolisGames.SDK.ads.showRewarded((success) => {
        if (success) {
            gameObject.SendMessage('OnRewardGranted', '1');
        }
    });
");
```

**After (Unity Plugin):**
```csharp
bool success = await SolisSDK.Ads.ShowRewardedAsync();

if (success)
{
    Debug.Log("Ad watched! Granting reward...");
    GivePlayerReward();
}
else
{
    Debug.Log("Ad was skipped or failed");
}
```

---

### 9. Tournaments

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval(@"
    window.SolisGames.SDK.tournaments.list({ status: 'active' })
        .then((tournaments) => {
            gameObject.SendMessage('OnTournamentsReceived', JSON.stringify(tournaments));
        });
");
```

**After (Unity Plugin):**
```csharp
var tournaments = await SolisSDK.Tournaments.ListAsync(status: "active");

foreach (var tournament in tournaments)
{
    Debug.Log($"🏆 {tournament.name}");
    Debug.Log($"   Participants: {tournament.participantCount}");
    Debug.Log($"   Prize: {tournament.prizePool} coins");
    Debug.Log($"   Ends: {tournament.endTime}");
}
```

---

### 10. Friends & Social

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval(@"
    window.SolisGames.SDK.friends.list().then((friends) => {
        gameObject.SendMessage('OnFriendsReceived', JSON.stringify(friends));
    });
");
```

**After (Unity Plugin):**
```csharp
var friends = await SolisSDK.Friends.ListAsync();

foreach (var friend in friends)
{
    Debug.Log($"👤 {friend.username} - {friend.status}");

    if (friend.currentGame != null)
    {
        Debug.Log($"   Playing: {friend.currentGame}");
    }
}
```

---

### 11. Chat

**Before (JavaScript SDK):**
```csharp
Application.ExternalEval($@"
    window.SolisGames.SDK.chat.send('lobby_1', 'Hello world!')
        .then(() => console.log('Message sent'));
");
```

**After (Unity Plugin):**
```csharp
bool success = await SolisSDK.Chat.SendMessageAsync("lobby_1", "Hello world!");

if (success)
{
    Debug.Log("Message sent!");
}
```

---

## Key Differences

### Error Handling

**Before (JavaScript SDK):**
- Errors lost in JavaScript console
- No try/catch support
- Generic error messages

**After (Unity Plugin):**
```csharp
try
{
    await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
}
catch (System.Exception ex)
{
    Debug.LogError($"Failed to submit score: {ex.Message}");
    ShowErrorToPlayer("Could not save your score. Please try again.");
}
```

---

### Type Safety

**Before (JavaScript SDK):**
```csharp
// No compile-time checking
Application.ExternalEval(@"
    window.SolisGames.SDK.leaderboards.submit('high_scors', { scoer: 1000 });
    // Typos cause runtime errors!
");
```

**After (Unity Plugin):**
```csharp
// Compile-time errors catch typos
await SolisSDK.Leaderboards.SubmitAsync("high_scors", 1000);
// ❌ Compiler error: method name wrong

await SolisSDK.Leaderboards.SubmitAsync("high_scores", "1000");
// ❌ Compiler error: wrong parameter type
```

---

### Editor Testing

**Before (JavaScript SDK):**
- Must build WebGL to test
- Build times: 5-15 minutes
- Slow iteration

**After (Unity Plugin):**
- Test in Play Mode (Editor)
- Mock data provided
- Instant iteration

```csharp
// Works in Editor without WebGL build!
async void Start()
{
    bool success = await SolisSDK.InitAsync("test-api-key");

    // Returns mock data in Editor, real data in WebGL build
    var user = await SolisSDK.User.GetUserAsync();
    Debug.Log($"User: {user.username}");
}
```

---

## Migration Checklist

- [ ] Install Unity Plugin via Package Manager
- [ ] Configure API key in SDK Settings window
- [ ] Remove JavaScript SDK initialization code
- [ ] Replace `Application.ExternalEval()` calls with Unity Plugin methods
- [ ] Convert callbacks to async/await
- [ ] Add error handling with try/catch
- [ ] Test in Unity Editor (Play Mode)
- [ ] Build WebGL and test on platform
- [ ] Remove custom .jslib files (if you had any)
- [ ] Update documentation/comments in code

---

## Common Pitfalls

### 1. Forgetting `await`
```csharp
// ❌ Wrong - fires and forgets
SolisSDK.Leaderboards.SubmitAsync("high_scores", score);

// ✅ Correct - waits for result
await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
```

### 2. Not Checking Initialization
```csharp
// ❌ Wrong - SDK not initialized yet
async void Start()
{
    SolisSDK.InitAsync("api-key"); // Not awaited!
    await SolisSDK.User.GetUserAsync(); // May fail!
}

// ✅ Correct - wait for initialization
async void Start()
{
    bool success = await SolisSDK.InitAsync("api-key");
    if (success)
    {
        await SolisSDK.User.GetUserAsync();
    }
}
```

### 3. Using `async void` Incorrectly
```csharp
// ❌ Wrong - exceptions not caught
async void SubmitScore()
{
    await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
}

// ✅ Correct - use async Task for error handling
async Task SubmitScore()
{
    try
    {
        await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Submit failed: {ex.Message}");
    }
}
```

---

## Performance Considerations

### JavaScript SDK
- Manual string concatenation
- No caching
- Frequent JS↔Unity marshalling overhead

### Unity Plugin
- Efficient binary marshalling
- Client-side caching built-in
- Optimized async operations

**Result:** Unity Plugin is ~2-3x faster for most operations.

---

## Getting Help

If you encounter issues during migration:

1. Check [Troubleshooting.md](Troubleshooting.md) for common issues
2. Review [API Reference](API-Reference.md) for correct method signatures
3. Check example projects in `Samples~/` folder
4. Join Discord: https://discord.gg/solisgames
5. Submit issue: https://github.com/solis-interactive/unity-sdk/issues

---

## Need More Help?

- **Documentation:** https://solisgames.com/docs/unity
- **Discord Community:** https://discord.gg/solisgames
- **GitHub Issues:** https://github.com/solis-interactive/unity-sdk/issues
- **Email Support:** support@solisgames.com
