# Solis Games Unity SDK

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS%2B-blue)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.md)
[![Platform](https://img.shields.io/badge/Platform-WebGL-orange)](https://unity.com/solutions/webgl)

Native C# SDK for Unity WebGL integration with [Solis Games](https://solisgames.com) platform.

## ✨ Features

- 🎮 **Native C# API** - Full IntelliSense support with async/await
- 📊 **Leaderboards & Tournaments** - Competitive gaming features built-in
- 🏆 **Achievements** - Xbox Live-style achievement system
- 👥 **Friends & Social** - Cross-game social features
- 💬 **In-Game Chat** - Real-time chat with auto-moderation
- 💰 **Monetization** - Ads (rewarded, interstitial, banner) and IAP
- 📈 **Analytics** - Event tracking and session management
- ☁️ **Cloud Saves** - Cross-device progress sync
- 🎨 **Unity Editor Integration** - Visual config window and one-click build
- 🧪 **Editor Fallbacks** - Test without WebGL builds using mock data

## 🚀 Quick Start

### Installation

**Option 1: Unity Package Manager (Recommended)**

1. Open Unity Editor
2. Go to `Window > Package Manager`
3. Click `+ > Add package from git URL`
4. Paste: `https://github.com/solis-interactive/unity-sdk.git`
5. Click `Add`

**Option 2: Download .unitypackage**

Download the latest [SolisGamesSDK.unitypackage](https://github.com/solis-interactive/unity-sdk/releases) and import it into your project.

### Basic Usage

```csharp
using UnityEngine;
using SolisGames;

public class GameManager : MonoBehaviour
{
    async void Start()
    {
        // Initialize SDK
        bool success = await SolisSDK.InitAsync("your-api-key");

        if (success)
        {
            Debug.Log("Solis Games SDK initialized!");

            // Submit score to leaderboard
            var result = await SolisSDK.Leaderboards.SubmitAsync("high_scores", 9500);
            Debug.Log($"Your rank: {result.rank}");

            // Unlock achievement
            await SolisSDK.Achievements.UnlockAsync("first_win");
        }
    }

    public async void OnRewardButtonClick()
    {
        bool rewarded = await SolisSDK.Ads.ShowRewardedAsync("reward_coins");

        if (rewarded)
        {
            // Give player reward
            GiveCoins(100);
        }
    }
}
```

### Configuration

1. Go to `Window > Solis Games > SDK Settings`
2. Enter your API key (get it from [Studio Dashboard](https://solisgames.com/studio))
3. Configure features (ads, analytics, cloud save, etc.)
4. Click `Save Settings`

## 📚 Documentation

- [Installation Guide](Documentation~/Installation.md)
- [Quick Start Tutorial](Documentation~/QuickStart.md)
- [API Reference](Documentation~/API-Reference.md)
- [Migration from JavaScript SDK](Documentation~/Migration-Guide.md)
- [Troubleshooting](Documentation~/Troubleshooting.md)
- [Examples](Documentation~/Examples.md)

## 🎯 Requirements

- Unity 2022.3 LTS or newer (including Unity 6+)
- WebGL build target
- Solis Games API key ([sign up here](https://solisgames.com/studio))

## 🤝 Support

- Documentation: [solisgames.com/docs/unity](https://solisgames.com/docs/unity)
- Issues: [GitHub Issues](https://github.com/solis-interactive/unity-sdk/issues)
- Email: support@solisgames.com

## 📄 License

MIT License - see [LICENSE.md](LICENSE.md) for details

## 🌟 Why Unity Plugin?

### Before (JavaScript SDK)
```csharp
// Manual JavaScript bridge setup
[DllImport("__Internal")]
private static extern void ShowRewardedAd(string placement);

void Start()
{
    ShowRewardedAd("coins"); // No return value, no async
}
```

### After (Unity Plugin) ✨
```csharp
// Native C# async API
async void Start()
{
    bool rewarded = await SolisSDK.Ads.ShowRewardedAsync("coins");
    if (rewarded) {
        GiveCoins(100);
    }
}
```

**Benefits:**
- ✅ IntelliSense autocomplete
- ✅ Compile-time type checking
- ✅ Zero JavaScript knowledge required
- ✅ Works in Unity Editor (mock data)
- ✅ One-click build & deploy
