# Troubleshooting Guide

Common issues and solutions for the Solis Games Unity SDK.

---

## Installation Issues

### Package Manager Can't Find Git URL

**Problem:**
```
Error: Cannot perform upm operation: Unable to add package [git URL]
```

**Solutions:**

1. **Check Git is installed:**
```bash
git --version
```
If not installed, download from https://git-scm.com/

2. **Check SSH keys are configured:**
```bash
git ls-remote https://github.com/solis-interactive/unity-sdk.git
```

3. **Use HTTPS instead of SSH:**
```
https://github.com/solis-interactive/unity-sdk.git
```

4. **Try manual UPM installation:**
   - Edit `Packages/manifest.json` directly
   - Add: `"com.solisgames.sdk": "https://github.com/solis-interactive/unity-sdk.git"`

---

### Assembly Definition Errors

**Problem:**
```
error CS0246: The type or namespace name 'SolisGames' could not be found
```

**Solutions:**

1. **Check assembly references:**
   - Your assembly must reference `SolisGamesSDK.Runtime`
   - Open your `.asmdef` file
   - Add to `references` array:
```json
{
  "references": [
    "SolisGamesSDK.Runtime"
  ]
}
```

2. **Reimport the package:**
   - Right-click `Packages/Solis Games SDK`
   - Select "Reimport"

3. **Restart Unity Editor**

---

## Initialization Issues

### SDK Initialization Fails in WebGL Build

**Problem:**
```
SDK initialization failed
```

**Solutions:**

1. **Check API key is configured:**
   - Window > Solis Games > SDK Settings
   - Verify API key is entered correctly
   - No extra spaces or line breaks

2. **Check SDK script is injected:**
   - Open `index.html` in your WebGL build folder
   - Search for `solis-games-sdk-v1.js`
   - Should see:
```html
<script src="https://solisgames.com/solis-games-sdk-v1.js"
        data-api-key="your-api-key"
        data-environment="production"></script>
```

3. **Manually re-inject SDK:**
   - Window > Solis Games > Re-inject SDK (Manual)
   - Select your WebGL build folder

4. **Check browser console for errors:**
   - Open DevTools (F12)
   - Look for errors in Console tab
   - Common issues:
     - Invalid API key
     - Network request blocked by CORS
     - SDK script failed to load

5. **Verify domain is whitelisted:**
   - Log into Studio Dashboard
   - Check allowed domains for your API key
   - Add your domain (e.g., `localhost:8000` for testing)

---

### SDK Initialization Works in Editor but Fails in Build

**Problem:**
SDK initializes successfully in Unity Editor but fails in WebGL build.

**Solution:**

This is expected behavior! The Unity Plugin uses **mock data in Editor** and **real API calls in WebGL builds**.

**In Editor:**
```csharp
// Returns mock data immediately
var user = await SolisSDK.User.GetUserAsync();
Debug.Log(user.username); // "DemoPlayer"
```

**In WebGL Build:**
```csharp
// Makes real API call
var user = await SolisSDK.User.GetUserAsync();
Debug.Log(user.username); // Actual player username
```

If initialization fails in WebGL but works in Editor:
- Check API key configuration
- Check network connectivity
- Check browser console for errors

---

## API Call Issues

### Methods Return Null or Default Values

**Problem:**
```csharp
var user = await SolisSDK.User.GetUserAsync();
Debug.Log(user.username); // Null or empty
```

**Solutions:**

1. **Check SDK is initialized:**
```csharp
async void Start()
{
    // ❌ Wrong - SDK not initialized
    var user = await SolisSDK.User.GetUserAsync();

    // ✅ Correct - initialize first
    bool success = await SolisSDK.InitAsync("api-key");
    if (success)
    {
        var user = await SolisSDK.User.GetUserAsync();
    }
}
```

2. **Check for exceptions:**
```csharp
try
{
    var user = await SolisSDK.User.GetUserAsync();
}
catch (System.Exception ex)
{
    Debug.LogError($"Failed: {ex.Message}");
}
```

3. **Check browser console:**
   - Open DevTools (F12)
   - Look for 401 Unauthorized or 404 Not Found errors

---

### "Task was cancelled" Exception

**Problem:**
```
System.Threading.Tasks.TaskCanceledException: A task was cancelled
```

**Solutions:**

1. **Increase timeout (for slow networks):**
```csharp
// Not currently configurable - contact support if needed
```

2. **Check network connectivity:**
   - Test API manually: https://solisgames.com/api/sdk/user
   - Verify you can reach the API from your browser

3. **Avoid destroying GameObject during async operation:**
```csharp
// ❌ Wrong - GameObject destroyed before task completes
async void Start()
{
    await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
}

void OnDestroy()
{
    // If called before SubmitAsync completes, task is cancelled
}

// ✅ Correct - handle cancellation
private CancellationTokenSource cts = new CancellationTokenSource();

async void Start()
{
    try
    {
        await SolisSDK.Leaderboards.SubmitAsync("high_scores", score);
    }
    catch (OperationCanceledException)
    {
        Debug.Log("Operation cancelled (expected on scene change)");
    }
}

void OnDestroy()
{
    cts.Cancel();
}
```

---

## Cloud Save Issues

### Load Returns Default/Empty Object

**Problem:**
```csharp
var save = await SolisSDK.CloudSave.LoadAsync<PlayerSave>("progress");
Debug.Log(save.level); // 0 (default value)
```

**Solutions:**

1. **Check if key exists:**
```csharp
// Save data first
await SolisSDK.CloudSave.SaveAsync("progress", playerSave);

// Then load
var loadedSave = await SolisSDK.CloudSave.LoadAsync<PlayerSave>("progress");
```

2. **Check key name matches:**
```csharp
// ❌ Wrong - different keys
await SolisSDK.CloudSave.SaveAsync("player_progress", save);
await SolisSDK.CloudSave.LoadAsync<PlayerSave>("progress"); // Different key!

// ✅ Correct - same key
await SolisSDK.CloudSave.SaveAsync("player_progress", save);
await SolisSDK.CloudSave.LoadAsync<PlayerSave>("player_progress");
```

3. **Check class is serializable:**
```csharp
// ❌ Wrong - not serializable
public class PlayerSave
{
    public int level;
}

// ✅ Correct - [System.Serializable] attribute
[System.Serializable]
public class PlayerSave
{
    public int level;
}
```

4. **Check for JSON serialization issues:**
```csharp
// Unity's JsonUtility doesn't support:
// - Dictionaries
// - Properties (only public fields)
// - Nested complex types

// ✅ Use supported types:
[System.Serializable]
public class PlayerSave
{
    public int level; // ✅ Public field
    public List<string> items; // ✅ List works
    // public Dictionary<string, int> stats; // ❌ Dictionary doesn't work
}
```

---

### Save Fails Silently

**Problem:**
```csharp
await SolisSDK.CloudSave.SaveAsync("progress", save); // Returns true but doesn't save
```

**Solutions:**

1. **Check return value:**
```csharp
bool success = await SolisSDK.CloudSave.SaveAsync("progress", save);
if (!success)
{
    Debug.LogError("Save failed!");
}
```

2. **Check data size (1MB limit):**
```csharp
string json = JsonUtility.ToJson(save);
int sizeBytes = System.Text.Encoding.UTF8.GetByteCount(json);
float sizeMB = sizeBytes / (1024f * 1024f);

Debug.Log($"Save data size: {sizeMB:F2} MB");

if (sizeMB > 1.0f)
{
    Debug.LogWarning("Save data exceeds 1MB limit!");
}
```

3. **Check for serialization errors:**
```csharp
try
{
    string json = JsonUtility.ToJson(save);
    Debug.Log($"Serialized JSON: {json}");
}
catch (System.Exception ex)
{
    Debug.LogError($"Serialization failed: {ex.Message}");
}
```

---

## Leaderboard Issues

### Scores Not Appearing on Leaderboard

**Problem:**
Score submission returns success but score doesn't appear on leaderboard.

**Solutions:**

1. **Check leaderboard key matches:**
```csharp
// ❌ Wrong - different keys
await SolisSDK.Leaderboards.SubmitAsync("high_scores", 1000);
await SolisSDK.Leaderboards.GetAsync("highscores"); // Different key!

// ✅ Correct - same key
await SolisSDK.Leaderboards.SubmitAsync("high_scores", 1000);
await SolisSDK.Leaderboards.GetAsync("high_scores");
```

2. **Check for anti-cheat flagging:**
```csharp
var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", 1000);

if (result.flaggedForReview)
{
    Debug.LogWarning($"Score flagged: {string.Join(", ", result.flags)}");
    // Score submitted but under review by anti-cheat system
}
```

3. **Check score is better than previous:**
   - Leaderboards only keep your **best** score
   - Submitting a lower score won't replace a higher one

4. **Wait for cache refresh:**
   - Leaderboards cache for 60 seconds
   - Wait 1-2 minutes and refresh

---

### "Rate limit exceeded" Error

**Problem:**
```
Error: Rate limit exceeded (max 10 submissions per minute)
```

**Solution:**

The leaderboard system rate-limits score submissions to prevent spam and cheating.

```csharp
// ❌ Wrong - submitting too frequently
for (int i = 0; i < 20; i++)
{
    await SolisSDK.Leaderboards.SubmitAsync("high_scores", i * 100);
}

// ✅ Correct - submit only when game ends
void OnGameEnd(float finalScore)
{
    await SolisSDK.Leaderboards.SubmitAsync("high_scores", finalScore);
}
```

**Limits:**
- 10 submissions per minute per player
- 100 submissions per hour per player

---

## Achievement Issues

### Achievement Unlocked But Not Showing

**Problem:**
```csharp
var achievement = await SolisSDK.Achievements.UnlockAsync("first_win");
Debug.Log(achievement); // Null
```

**Solutions:**

1. **Check achievement exists in Studio Dashboard:**
   - Log into Studio Dashboard
   - Go to Achievements section
   - Verify achievement ID exists

2. **Check achievement is already unlocked:**
```csharp
var stats = await SolisSDK.Achievements.GetStatsAsync();
Debug.Log($"Total unlocked: {stats.totalUnlocked}");

// Get list of unlocked achievements
var unlocked = stats.unlockedAchievements;
if (unlocked.Contains("first_win"))
{
    Debug.Log("Achievement already unlocked!");
}
```

3. **Check for exceptions:**
```csharp
try
{
    var achievement = await SolisSDK.Achievements.UnlockAsync("first_win");
}
catch (System.Exception ex)
{
    Debug.LogError($"Unlock failed: {ex.Message}");
}
```

---

## Ad Issues

### Ads Not Showing

**Problem:**
```csharp
bool success = await SolisSDK.Ads.ShowRewardedAsync();
Debug.Log(success); // False
```

**Solutions:**

1. **Check ads are enabled in SDK Settings:**
   - Window > Solis Games > SDK Settings
   - Verify "Ads" is enabled

2. **Check ad frequency limit:**
   - Rewarded ads: Max 1 per 60 seconds
   - Interstitial ads: Max 1 per 180 seconds

3. **Check ad blocker:**
   - Disable browser ad blockers
   - Test in incognito/private mode

4. **Check ad inventory:**
   - Ads may not always be available
   - Handle `success = false` gracefully:

```csharp
bool adShown = await SolisSDK.Ads.ShowRewardedAsync();

if (adShown)
{
    GivePlayerReward();
}
else
{
    ShowMessage("No ads available right now. Try again later!");
}
```

---

### Ad Shows But Reward Not Granted

**Problem:**
Player watches ad but reward isn't given.

**Solution:**

Always check the return value:

```csharp
// ❌ Wrong - not checking result
await SolisSDK.Ads.ShowRewardedAsync();
GivePlayerReward(); // Always gives reward!

// ✅ Correct - check if ad was watched
bool adWatched = await SolisSDK.Ads.ShowRewardedAsync();

if (adWatched)
{
    GivePlayerReward();
    Debug.Log("Reward granted!");
}
else
{
    Debug.Log("Ad was skipped or failed");
}
```

---

## Build Issues

### WebGL Build Fails

**Problem:**
```
Build failed with errors
```

**Solutions:**

1. **Check Unity version:**
   - Minimum: Unity 2022.3 LTS
   - Recommended: Unity 6 or later

2. **Check WebGL module is installed:**
   - Unity Hub > Installs > [Your Unity Version] > Add Modules
   - Install "WebGL Build Support"

3. **Check project settings:**
   - File > Build Settings > WebGL
   - Player Settings > Resolution and Presentation
   - Set compression format (Gzip recommended)

4. **Clear build cache:**
   - Delete `Library/` folder
   - File > Build Settings > Clean Build

---

### SDK Script Not Injected in Build

**Problem:**
Opening `index.html` shows: `SolisGames is not defined`

**Solutions:**

1. **Check post-build script ran:**
   - Look for console message: "✅ SDK script injected successfully!"

2. **Manually re-inject:**
   - Window > Solis Games > Re-inject SDK (Manual)
   - Select your WebGL build folder

3. **Check index.html:**
```html
<!-- Should contain this in <head>: -->
<script src="https://solisgames.com/solis-games-sdk-v1.js"
        data-api-key="your-api-key"
        data-environment="production"></script>
```

4. **Manual injection (last resort):**
   - Open `index.html` in text editor
   - Add script tag before `</head>`
   - Save and test

---

## Runtime Errors

### "SendMessage has no receiver" Warning

**Problem:**
```
SendMessage OnUserReceived has no receiver!
```

**Solution:**

This warning is harmless. It occurs when the JavaScript SDK calls a Unity method before the GameObject is ready. The SDK handles this internally.

To suppress:
```csharp
// Warnings are logged to console but don't affect functionality
// No action needed
```

---

### IL2CPP Build Errors

**Problem:**
```
IL2CPP build failed
```

**Solutions:**

1. **Check callback attributes:**
   - All callbacks must have `[AOT.MonoPInvokeCallback]`
   - SDK already includes these - no action needed

2. **Check Managed Stripping Level:**
   - File > Build Settings > Player Settings
   - Other Settings > Managed Stripping Level
   - Try "Low" instead of "High"

3. **Add link.xml to preserve types:**
```xml
<linker>
  <assembly fullname="SolisGamesSDK.Runtime" preserve="all"/>
</linker>
```

---

## Performance Issues

### Long Loading Times

**Problem:**
Game takes 30+ seconds to initialize.

**Solutions:**

1. **Check network speed:**
   - SDK requires initial API call to initialize
   - Slow networks = slow initialization

2. **Show loading screen:**
```csharp
async void Start()
{
    ShowLoadingScreen();

    bool success = await SolisSDK.InitAsync("api-key");

    HideLoadingScreen();
}
```

3. **Initialize early:**
```csharp
// Initialize in first scene (splash screen)
// So it's ready when game starts
```

---

### High Memory Usage

**Problem:**
Game uses more memory than expected.

**Solutions:**

1. **Check cloud save data size:**
   - Large save files consume memory
   - Keep saves under 100KB

2. **Limit leaderboard queries:**
```csharp
// ❌ Wrong - fetching too much data
var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 10000);

// ✅ Correct - fetch only what you need
var rankings = await SolisSDK.Leaderboards.GetAsync("high_scores", limit: 100);
```

---

## Network Issues

### "Failed to fetch" Error

**Problem:**
```
TypeError: Failed to fetch
```

**Solutions:**

1. **Check CORS settings:**
   - API requests must come from whitelisted domains
   - Add your domain in Studio Dashboard

2. **Check SSL certificate:**
   - Must use HTTPS (not HTTP) in production
   - `localhost` is allowed for testing

3. **Check firewall/antivirus:**
   - Some antivirus software blocks API requests
   - Whitelist `solisgames.com`

---

### Requests Timing Out

**Problem:**
API requests take forever and eventually fail.

**Solutions:**

1. **Check network connectivity:**
```bash
ping solisgames.com
```

2. **Test API manually:**
   - Open https://solisgames.com/api/sdk/user in browser
   - Should see JSON response

3. **Check CDN status:**
   - Visit https://status.solisgames.com
   - Check for ongoing incidents

---

## Getting More Help

If your issue isn't listed here:

1. **Check API Reference:**
   - [API-Reference.md](API-Reference.md) - Complete API documentation

2. **Check Examples:**
   - [Examples.md](Examples.md) - Code examples
   - `Samples~/QuickStart/` - Minimal example project
   - `Samples~/FullFeatures/` - Complete integration example

3. **Community Support:**
   - Discord: https://discord.gg/TZdBFBhW
   - GitHub Issues: https://github.com/solis-interactive/unity-sdk/issues

4. **Contact Support:**
   - Email: support@solisgames.com
   - Include:
     - Unity version
     - SDK version
     - Error messages
     - Steps to reproduce

---

## Useful Debug Commands

### Check SDK Version
```csharp
Debug.Log($"SDK Version: {SolisSDK.Version}");
```

### Check Initialization Status
```csharp
Debug.Log($"SDK Initialized: {SolisSDK.IsInitialized}");
```

### Check API Key
```csharp
// In Editor
string apiKey = UnityEditor.EditorPrefs.GetString("SolisGames_ApiKey");
Debug.Log($"API Key: {apiKey.Substring(0, 8)}...");
```

### Check Network Connectivity
```csharp
// Open browser DevTools (F12)
// Network tab > Filter by "solisgames.com"
// Check for failed requests
```

### Enable Verbose Logging
```csharp
// Add to your initialization code
SolisSDK.EnableDebugLogging = true; // If implemented in future version
```

---

**Still stuck?** Open a support ticket with:
- Unity version
- SDK version
- Platform (WebGL)
- Error message or screenshot
- Steps to reproduce

We're here to help! 🚀
