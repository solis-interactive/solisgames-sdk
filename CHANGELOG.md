# Changelog

All notable changes to the Solis Games Unity SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-XX

### Added
- Initial release of Solis Games Unity SDK
- Native C# API with full async/await support
- Unity Editor integration with visual config window
- Automatic SDK script injection for WebGL builds
- Editor fallbacks with mock data for testing

#### Core Features
- **SDK Initialization** - `SolisSDK.InitAsync()`
- **User Management** - Get user data, check authentication

#### Monetization
- **Ads** - Rewarded, interstitial, and banner ads
  - `SolisSDK.Ads.ShowRewardedAsync()`
  - `SolisSDK.Ads.ShowInterstitialAsync()`
  - `SolisSDK.Ads.ShowBannerAsync()`

#### Analytics
- **Event Tracking** - Custom event tracking
  - `SolisSDK.Analytics.TrackEventAsync()`
  - `SolisSDK.Analytics.StartSessionAsync()`

#### Cloud Storage
- **Cloud Saves** - Cross-device progress sync with generic types
  - `SolisSDK.CloudSave.SaveAsync<T>()`
  - `SolisSDK.CloudSave.LoadAsync<T>()`

#### Competitive Gaming
- **Leaderboards** - Real-time rankings with anti-cheat
  - `SolisSDK.Leaderboards.SubmitAsync()`
  - `SolisSDK.Leaderboards.GetAsync()`
  - `SolisSDK.Leaderboards.GetNearbyAsync()`

- **Tournaments** - Automated tournament system
  - `SolisSDK.Tournaments.JoinAsync()`
  - `SolisSDK.Tournaments.ListAsync()`
  - `SolisSDK.Tournaments.GetBracketAsync()`

- **Achievements** - Xbox Live-style achievements
  - `SolisSDK.Achievements.UnlockAsync()`
  - `SolisSDK.Achievements.UpdateProgressAsync()`
  - `SolisSDK.Achievements.ListAsync()`

#### Social Features
- **Friends System** - Cross-game friends and presence
  - `SolisSDK.Friends.ListAsync()`
  - `SolisSDK.Friends.AddAsync()`
  - `SolisSDK.Friends.GetOnlineAsync()`
  - `SolisSDK.Friends.UpdatePresenceAsync()`

- **Chat** - Real-time chat with auto-moderation
  - `SolisSDK.Chat.SendAsync()`
  - `SolisSDK.Chat.GetHistoryAsync()`
  - `SolisSDK.Chat.SubscribeToChannel()`

#### Unity Editor Tools
- SDK Settings window (`Window > Solis Games > SDK Settings`)
- Build & Deploy window (`Window > Solis Games > Build & Deploy`)
- Post-build script for automatic SDK injection

#### Documentation
- Installation guide with 3 installation methods
- Quick start tutorial (5 minutes to first integration)
- Full API reference with code examples
- Migration guide from JavaScript SDK
- Troubleshooting guide
- Example projects (QuickStart, FullFeatures, 2DPlatformer)

### Requirements
- Unity 2022.3 LTS or newer
- WebGL build target
- Solis Games API key

### Known Issues
- None

---

## Future Releases

### [1.1.0] - Planned
- WebGPU-specific features (Unity 6.0+)
- GPU compute integration
- Enhanced error handling and logging
- Performance optimizations

### [1.2.0] - Planned
- One-click deploy to Solis Games platform
- In-editor analytics dashboard
- Advanced debugging tools
