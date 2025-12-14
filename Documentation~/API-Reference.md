# API Reference

Complete API documentation for the Solis Games Unity SDK.

## Table of Contents

- [Core SDK](#core-sdk)
- [User Module](#user-module)
- [Ads Module](#ads-module)
- [Analytics Module](#analytics-module)
- [Cloud Save Module](#cloud-save-module)
- [Leaderboards Module](#leaderboards-module)
- [Tournaments Module](#tournaments-module)
- [Achievements Module](#achievements-module)
- [Friends Module](#friends-module)
- [Chat Module](#chat-module)

---

## Core SDK

### `SolisSDK.InitAsync()`

Initialize the Solis Games SDK. Must be called before using any other SDK features.

**Signature:**
```csharp
Task<bool> InitAsync(string apiKey, string gameId = null)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `apiKey` | string | Yes | Your API key from Studio Dashboard |
| `gameId` | string | No | Optional game ID (auto-detected if not provided) |

**Returns:** `Task<bool>` - True if initialization succeeded

**Example:**
```csharp
bool success = await SolisSDK.InitAsync("your-api-key");

if (success)
{
    Debug.Log("SDK initialized!");
}
```

**Notes:**
- Call this once at game startup
- In Unity Editor, uses mock data for testing
- In WebGL builds, connects to Solis Games platform

---

### `SolisSDK.IsInitialized`

Check if the SDK has been initialized.

**Signature:**
```csharp
bool IsInitialized { get; }
```

**Example:**
```csharp
if (SolisSDK.IsInitialized)
{
    // SDK is ready
}
```

---

## User Module

Access via `SolisSDK.User`

### `GetAsync()`

Get current user profile information.

**Signature:**
```csharp
Task<UserData> GetAsync()
```

**Returns:** `Task<UserData>` - User profile data

**UserData Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `id` | string | User UUID |
| `username` | string | Display name |
| `email` | string | Email address |
| `avatar_url` | string | Profile picture URL |
| `is_authenticated` | bool | Authentication status |
| `total_xp` | int | Total XP earned |
| `level` | int | Player level |
| `created_at` | string | Account creation date (ISO 8601) |

**Example:**
```csharp
var user = await SolisSDK.User.GetAsync();

Debug.Log($"Welcome, {user.username}!");
Debug.Log($"Level: {user.level} (XP: {user.total_xp})");
```

---

## Ads Module

Access via `SolisSDK.Ads`

### `ShowRewardedAsync()`

Show a rewarded ad. Player must watch to receive reward.

**Signature:**
```csharp
Task<bool> ShowRewardedAsync(string placement = "default")
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `placement` | string | No | "default" | Ad placement ID |

**Returns:** `Task<bool>` - True if player watched ad and should receive reward

**Example:**
```csharp
bool rewarded = await SolisSDK.Ads.ShowRewardedAsync("reward_coins");

if (rewarded)
{
    // Give player reward
    PlayerCoins += 100;
    Debug.Log("Reward granted!");
}
else
{
    Debug.Log("Player did not watch ad");
}
```

---

### `ShowInterstitialAsync()`

Show a full-screen interstitial ad.

**Signature:**
```csharp
Task<bool> ShowInterstitialAsync(string placement = "default")
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `placement` | string | No | "default" | Ad placement ID |

**Returns:** `Task<bool>` - True if ad was shown successfully

**Example:**
```csharp
// Show ad between levels
bool shown = await SolisSDK.Ads.ShowInterstitialAsync("level_complete");

if (shown)
{
    Debug.Log("Ad shown successfully");
}
```

---

### `ShowBannerAsync()`

Show a banner ad at top or bottom of screen.

**Signature:**
```csharp
Task<bool> ShowBannerAsync(string placement = "default", string position = "bottom")
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `placement` | string | No | "default" | Ad placement ID |
| `position` | string | No | "bottom" | Position: "top" or "bottom" |

**Returns:** `Task<bool>` - True if banner was shown successfully

**Example:**
```csharp
// Show banner at bottom
await SolisSDK.Ads.ShowBannerAsync("main_menu", "bottom");
```

---

### `HideBanner()`

Hide the currently displayed banner ad.

**Signature:**
```csharp
void HideBanner()
```

**Example:**
```csharp
// Hide banner when entering gameplay
SolisSDK.Ads.HideBanner();
```

---

## Analytics Module

Access via `SolisSDK.Analytics`

### `TrackEventAsync()`

Track a custom analytics event.

**Signature:**
```csharp
Task<bool> TrackEventAsync(string eventName, Dictionary<string, object> eventData = null)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `eventName` | string | Yes | Event identifier (e.g., "level_complete") |
| `eventData` | Dictionary<string, object> | No | Optional event metadata |

**Returns:** `Task<bool>` - True if event was tracked successfully

**Example:**
```csharp
// Track level completion with metadata
await SolisSDK.Analytics.TrackEventAsync("level_complete", new Dictionary<string, object>
{
    { "level", 5 },
    { "time", 120.5f },
    { "score", 9500 },
    { "stars", 3 }
});

// Track simple event
await SolisSDK.Analytics.TrackEventAsync("game_start");
```

**Common Event Names:**
- `game_start` - Player started the game
- `level_complete` - Player completed a level
- `level_fail` - Player failed a level
- `purchase` - Player made a purchase
- `tutorial_complete` - Player finished tutorial
- `achievement_unlock` - Player unlocked achievement

---

### `StartSessionAsync()`

Start an analytics session. Usually called automatically on SDK init.

**Signature:**
```csharp
Task<bool> StartSessionAsync()
```

**Returns:** `Task<bool>` - True if session started successfully

---

### `EndSessionAsync()`

End an analytics session. Automatically called on app quit.

**Signature:**
```csharp
Task<bool> EndSessionAsync()
```

**Returns:** `Task<bool>` - True if session ended successfully

---

## Cloud Save Module

Access via `SolisSDK.CloudSave`

### `SaveAsync<T>()`

Save data to cloud storage with automatic cross-device sync.

**Signature:**
```csharp
Task<bool> SaveAsync<T>(string key, T data)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `key` | string | Yes | Save slot identifier |
| `data` | T | Yes | Data to save (must be serializable) |

**Returns:** `Task<bool>` - True if save succeeded

**Example:**
```csharp
[System.Serializable]
public class PlayerSaveData
{
    public int level = 1;
    public int coins = 0;
    public string[] unlockedLevels = new string[0];
}

// Save game
var saveData = new PlayerSaveData
{
    level = currentLevel,
    coins = playerCoins,
    unlockedLevels = new string[] { "level_1", "level_2" }
};

bool success = await SolisSDK.CloudSave.SaveAsync("save_slot_1", saveData);

if (success)
{
    Debug.Log("Game saved!");
}
```

---

### `LoadAsync<T>()`

Load data from cloud storage.

**Signature:**
```csharp
Task<T> LoadAsync<T>(string key)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `key` | string | Yes | Save slot identifier |

**Returns:** `Task<T>` - Loaded data, or `default(T)` if not found

**Example:**
```csharp
// Load game
var saveData = await SolisSDK.CloudSave.LoadAsync<PlayerSaveData>("save_slot_1");

if (saveData != null)
{
    currentLevel = saveData.level;
    playerCoins = saveData.coins;
    Debug.Log($"Game loaded! Level: {saveData.level}");
}
else
{
    Debug.Log("No save data found, starting new game");
}
```

**Notes:**
- Data limit: 1MB per save slot
- Data is automatically synced across devices
- Use different keys for multiple save slots: `"slot_1"`, `"slot_2"`, etc.

---

## Leaderboards Module

Access via `SolisSDK.Leaderboards`

### `SubmitAsync()`

Submit a score to a leaderboard.

**Signature:**
```csharp
Task<LeaderboardSubmitResult> SubmitAsync(string leaderboardKey, float score, Dictionary<string, object> metadata = null)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `leaderboardKey` | string | Yes | Leaderboard identifier (e.g., "high_scores") |
| `score` | float | Yes | Score value |
| `metadata` | Dictionary<string, object> | No | Optional metadata (level, time, etc.) |

**Returns:** `Task<LeaderboardSubmitResult>`

**LeaderboardSubmitResult Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `success` | bool | Whether submission succeeded |
| `rank` | int | Player's rank after submission |
| `score` | float | Submitted score |
| `leaderboard_key` | string | Leaderboard identifier |
| `flagged_for_review` | bool | If score was flagged by anti-cheat |

**Example:**
```csharp
// Submit score with metadata
var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", 9500, new Dictionary<string, object>
{
    { "level", 10 },
    { "time", 120.5f }
});

if (result != null && result.success)
{
    Debug.Log($"Score submitted! Your rank: #{result.rank}");

    if (result.flagged_for_review)
    {
        Debug.LogWarning("Score flagged for review");
    }
}
```

---

### `GetAsync()`

Get leaderboard rankings.

**Signature:**
```csharp
Task<LeaderboardRankings> GetAsync(string leaderboardKey, string scope = "global", int limit = 100, int offset = 0)
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `leaderboardKey` | string | Yes | - | Leaderboard identifier |
| `scope` | string | No | "global" | Scope: "global", "daily", "weekly" |
| `limit` | int | No | 100 | Number of entries to fetch |
| `offset` | int | No | 0 | Offset for pagination |

**Returns:** `Task<LeaderboardRankings>`

**LeaderboardRankings Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `entries` | List<LeaderboardEntry> | List of leaderboard entries |
| `total_count` | int | Total number of entries |
| `leaderboard_key` | string | Leaderboard identifier |
| `scope` | string | Scope used |

**LeaderboardEntry Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `rank` | int | Player rank |
| `user_id` | string | Player UUID |
| `username` | string | Player name |
| `avatar_url` | string | Player avatar |
| `score` | float | Player score |
| `created_at` | string | Submission timestamp |

**Example:**
```csharp
// Get top 10 global rankings
var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 10);

if (rankings != null && rankings.entries != null)
{
    Debug.Log($"Top {rankings.entries.Count} players:");

    foreach (var entry in rankings.entries)
    {
        Debug.Log($"#{entry.rank} - {entry.username}: {entry.score}");
    }
}

// Get daily rankings with pagination
var dailyRankings = await SolisSDK.Leaderboards.GetAsync(
    "high_scores",
    scope: "daily",
    limit: 50,
    offset: 0
);
```

---

### `GetNearbyAsync()`

Get leaderboard entries near the current player's rank.

**Signature:**
```csharp
Task<LeaderboardRankings> GetNearbyAsync(string leaderboardKey, int range = 5)
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `leaderboardKey` | string | Yes | - | Leaderboard identifier |
| `range` | int | No | 5 | Number of ranks above/below player |

**Returns:** `Task<LeaderboardRankings>` - Rankings centered on player

**Example:**
```csharp
// Get 5 ranks above and below player (11 total entries)
var nearby = await SolisSDK.Leaderboards.GetNearbyAsync("high_scores", range: 5);

if (nearby != null && nearby.entries != null)
{
    foreach (var entry in nearby.entries)
    {
        bool isPlayer = entry.user_id == myUserId;
        string marker = isPlayer ? " ← YOU" : "";
        Debug.Log($"#{entry.rank} - {entry.username}: {entry.score}{marker}");
    }
}
```

---

## Tournaments Module

Access via `SolisSDK.Tournaments`

### `JoinAsync()`

Join a tournament.

**Signature:**
```csharp
Task<bool> JoinAsync(string tournamentId)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tournamentId` | string | Yes | Tournament UUID |

**Returns:** `Task<bool>` - True if successfully joined

**Example:**
```csharp
bool joined = await SolisSDK.Tournaments.JoinAsync("tournament-uuid-here");

if (joined)
{
    Debug.Log("Successfully joined tournament!");
}
```

---

### `ListAsync()`

Get list of tournaments.

**Signature:**
```csharp
Task<TournamentList> ListAsync(string status = "all")
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `status` | string | No | "all" | Filter: "active", "pending", "completed", "all" |

**Returns:** `Task<TournamentList>`

**Tournament Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `id` | string | Tournament UUID |
| `name` | string | Tournament name |
| `description` | string | Tournament description |
| `status` | string | Status: pending/active/completed |
| `bracket_type` | string | Type: single_elimination, etc. |
| `start_time` | string | Start time (ISO 8601) |
| `end_time` | string | End time (ISO 8601) |
| `max_participants` | int | Maximum players |
| `participant_count` | int | Current participant count |
| `is_participant` | bool | If current player joined |

**Example:**
```csharp
// Get active tournaments
var tournamentList = await SolisSDK.Tournaments.ListAsync("active");

if (tournamentList != null && tournamentList.tournaments != null)
{
    foreach (var tournament in tournamentList.tournaments)
    {
        Debug.Log($"{tournament.name}: {tournament.participant_count}/{tournament.max_participants} players");

        if (!tournament.is_participant)
        {
            // Show join button
        }
    }
}
```

---

### `GetBracketAsync()`

Get tournament bracket with all matches.

**Signature:**
```csharp
Task<TournamentBracket> GetBracketAsync(string tournamentId)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tournamentId` | string | Yes | Tournament UUID |

**Returns:** `Task<TournamentBracket>`

**TournamentBracket Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `tournament_id` | string | Tournament UUID |
| `matches` | List<TournamentMatch> | All matches in bracket |

**TournamentMatch Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `round` | int | Round number |
| `player1_username` | string | Player 1 name |
| `player2_username` | string | Player 2 name |
| `player1_score` | float | Player 1 score |
| `player2_score` | float | Player 2 score |
| `winner_id` | string | Winner UUID |
| `status` | string | pending/in_progress/completed |

**Example:**
```csharp
var bracket = await SolisSDK.Tournaments.GetBracketAsync("tournament-uuid");

if (bracket != null && bracket.matches != null)
{
    // Group by round
    var rounds = bracket.matches.GroupBy(m => m.round).OrderBy(g => g.Key);

    foreach (var round in rounds)
    {
        Debug.Log($"Round {round.Key}:");

        foreach (var match in round)
        {
            Debug.Log($"  {match.player1_username} vs {match.player2_username}");

            if (match.status == "completed")
            {
                Debug.Log($"  Winner: {match.winner_id}");
            }
        }
    }
}
```

---

## Achievements Module

Access via `SolisSDK.Achievements`

### `UnlockAsync()`

Unlock an achievement.

**Signature:**
```csharp
Task<bool> UnlockAsync(string achievementId)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `achievementId` | string | Yes | Achievement identifier |

**Returns:** `Task<bool>` - True if unlocked successfully

**Example:**
```csharp
// Unlock achievement when player completes first level
if (levelNumber == 1 && completed)
{
    bool unlocked = await SolisSDK.Achievements.UnlockAsync("first_level");

    if (unlocked)
    {
        Debug.Log("Achievement unlocked!");
    }
}
```

---

### `UpdateProgressAsync()`

Update progress towards an achievement (for achievements that track progress).

**Signature:**
```csharp
Task<bool> UpdateProgressAsync(string achievementId, int current, int target)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `achievementId` | string | Yes | Achievement identifier |
| `current` | int | Yes | Current progress value |
| `target` | int | Yes | Target progress value |

**Returns:** `Task<bool>` - True if updated successfully

**Example:**
```csharp
// Track progress: "Collect 100 coins"
int coinsCollected = 75;
int coinsNeeded = 100;

bool updated = await SolisSDK.Achievements.UpdateProgressAsync(
    "collect_100_coins",
    coinsCollected,
    coinsNeeded
);

// Progress is now 75%
```

---

### `ListAsync()`

Get all achievements for the game with unlock status.

**Signature:**
```csharp
Task<AchievementList> ListAsync()
```

**Returns:** `Task<AchievementList>`

**Achievement Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `id` | string | Achievement ID |
| `name` | string | Achievement name |
| `description` | string | Achievement description |
| `icon_url` | string | Achievement icon |
| `rarity` | string | Rarity: common/uncommon/rare/epic/legendary |
| `xp_reward` | int | XP awarded on unlock |
| `unlocked` | bool | If player unlocked it |
| `unlocked_at` | string | Unlock timestamp |
| `progress` | int | Progress percentage (0-100) |
| `unlock_rate` | float | % of players who unlocked |

**Example:**
```csharp
var achievementList = await SolisSDK.Achievements.ListAsync();

if (achievementList != null && achievementList.achievements != null)
{
    int unlockedCount = achievementList.achievements.Count(a => a.unlocked);
    int totalCount = achievementList.achievements.Count;

    Debug.Log($"Achievements: {unlockedCount}/{totalCount} unlocked");

    foreach (var achievement in achievementList.achievements)
    {
        string status = achievement.unlocked
            ? $"✓ Unlocked"
            : $"{achievement.progress}%";

        Debug.Log($"{achievement.name} ({achievement.rarity}): {status}");
    }
}
```

---

## Friends Module

Access via `SolisSDK.Friends`

### `ListAsync()`

Get friends list with presence information.

**Signature:**
```csharp
Task<List<Friend>> ListAsync()
```

**Returns:** `Task<List<Friend>>`

**Friend Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `id` | string | Friend UUID |
| `username` | string | Friend name |
| `avatar_url` | string | Friend avatar |
| `presence` | FriendPresence | Presence information |

**FriendPresence Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `status` | string | online/away/offline/playing |
| `game_title` | string | Currently playing game |
| `custom_status` | string | Custom status message |
| `joinable` | bool | Can join their session |

**Example:**
```csharp
var friends = await SolisSDK.Friends.ListAsync();

if (friends != null)
{
    Debug.Log($"You have {friends.Count} friends");

    foreach (var friend in friends)
    {
        string presenceStr = friend.presence?.status ?? "offline";

        if (friend.presence?.game_title != null)
        {
            presenceStr += $" - Playing {friend.presence.game_title}";
        }

        Debug.Log($"{friend.username}: {presenceStr}");
    }
}
```

---

### `AddAsync()`

Send a friend request.

**Signature:**
```csharp
Task<bool> AddAsync(string userId)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `userId` | string | Yes | User ID to add as friend |

**Returns:** `Task<bool>` - True if request sent successfully

**Example:**
```csharp
bool sent = await SolisSDK.Friends.AddAsync("user-uuid-here");

if (sent)
{
    Debug.Log("Friend request sent!");
}
```

---

### `RemoveAsync()`

Remove a friend.

**Signature:**
```csharp
Task<bool> RemoveAsync(string userId)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `userId` | string | Yes | Friend's user ID |

**Returns:** `Task<bool>` - True if removed successfully

---

### `GetOnlineAsync()`

Get only online friends.

**Signature:**
```csharp
Task<List<Friend>> GetOnlineAsync()
```

**Returns:** `Task<List<Friend>>` - List of online friends

**Example:**
```csharp
var onlineFriends = await SolisSDK.Friends.GetOnlineAsync();

Debug.Log($"{onlineFriends.Count} friends online");
```

---

### `UpdatePresenceAsync()`

Update your presence status.

**Signature:**
```csharp
Task<bool> UpdatePresenceAsync(string status, string gameTitle = null, string customStatus = null, bool joinable = false)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `status` | string | Yes | Status: online/away/offline/playing |
| `gameTitle` | string | No | Game title (when playing) |
| `customStatus` | string | No | Custom status message |
| `joinable` | bool | No | Can friends join your session |

**Returns:** `Task<bool>` - True if updated successfully

**Example:**
```csharp
// Set status to playing
await SolisSDK.Friends.UpdatePresenceAsync(
    "playing",
    gameTitle: "Space Raiders",
    customStatus: "Level 10 Boss Fight",
    joinable: true
);

// Set status to away
await SolisSDK.Friends.UpdatePresenceAsync("away");
```

---

## Chat Module

Access via `SolisSDK.Chat`

### `SendAsync()`

Send a chat message.

**Signature:**
```csharp
Task<bool> SendAsync(string channelId, string message)
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `channelId` | string | Yes | Chat channel ID |
| `message` | string | Yes | Message text |

**Returns:** `Task<bool>` - True if sent successfully

**Example:**
```csharp
bool sent = await SolisSDK.Chat.SendAsync("lobby_1", "GG everyone!");

if (sent)
{
    Debug.Log("Message sent!");
}
```

---

### `GetHistoryAsync()`

Get chat history for a channel.

**Signature:**
```csharp
Task<List<ChatMessage>> GetHistoryAsync(string channelId, int limit = 50)
```

**Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `channelId` | string | Yes | - | Chat channel ID |
| `limit` | int | No | 50 | Number of messages to fetch |

**Returns:** `Task<List<ChatMessage>>`

**ChatMessage Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `id` | string | Message UUID |
| `channel_id` | string | Channel ID |
| `user_id` | string | Sender UUID |
| `username` | string | Sender name |
| `avatar_url` | string | Sender avatar |
| `message` | string | Message text |
| `created_at` | string | Timestamp (ISO 8601) |
| `flagged` | bool | If flagged by moderation |
| `flag_reason` | string | Moderation flag reason |

**Example:**
```csharp
var messages = await SolisSDK.Chat.GetHistoryAsync("lobby_1", limit: 50);

if (messages != null)
{
    foreach (var msg in messages)
    {
        Debug.Log($"{msg.username}: {msg.message}");

        if (msg.flagged)
        {
            Debug.LogWarning($"Flagged message: {msg.flag_reason}");
        }
    }
}
```

---

## Error Handling

All async methods can throw exceptions. Use try-catch for error handling:

```csharp
try
{
    var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", 9500);
}
catch (System.Exception ex)
{
    Debug.LogError($"Failed to submit score: {ex.Message}");
}
```

Common error scenarios:
- SDK not initialized → Check `SolisSDK.IsInitialized` first
- Network errors → Retry with exponential backoff
- Invalid parameters → Validate inputs before calling SDK methods

---

## Next Steps

- [Examples](Examples.md) - More code examples
- [Troubleshooting](Troubleshooting.md) - Common issues
- [Migration Guide](Migration-Guide.md) - Migrate from JavaScript SDK

---

**Need help?** Open an issue on [GitHub](https://github.com/solis-interactive/unity-sdk/issues)
