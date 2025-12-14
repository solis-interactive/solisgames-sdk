# Installation Guide

This guide will walk you through installing the Solis Games Unity SDK in your project.

## Prerequisites

- Unity 2022.3 LTS or newer (including Unity 6+)
- WebGL build target installed
- Solis Games API key ([get yours here](https://solisgames.com/studio))

## Installation Methods

Choose one of the following installation methods:

---

## Method 1: Unity Package Manager (Git URL) - **Recommended**

This is the easiest way to install and keep the SDK up to date.

### Steps:

1. Open your Unity project
2. Go to `Window > Package Manager`
3. Click the `+` button in the top-left corner
4. Select `Add package from git URL...`
5. Paste the following URL:
   ```
   https://github.com/solis-interactive/unity-sdk.git
   ```
6. Click `Add`
7. Wait for Unity to download and import the package

### Updating:

To update to the latest version:
1. Go to `Window > Package Manager`
2. Find "Solis Games SDK" in the list
3. Click `Update` if a new version is available

---

## Method 2: Unity Asset Store

### Steps:

1. Open Unity Editor
2. Go to `Window > Asset Store`
3. Search for "Solis Games SDK"
4. Click `Download`
5. Click `Import` after download completes
6. In the import dialog, ensure all files are selected
7. Click `Import`

---

## Method 3: Download .unitypackage

### Steps:

1. Download the latest `SolisGamesSDK.unitypackage` from [GitHub Releases](https://github.com/solis-interactive/unity-sdk/releases)
2. In Unity, go to `Assets > Import Package > Custom Package...`
3. Navigate to the downloaded `.unitypackage` file
4. Click `Open`
5. In the import dialog, ensure all files are selected
6. Click `Import`

**Alternative:** Double-click the `.unitypackage` file to import directly.

---

## Verify Installation

After installation, verify the SDK is installed correctly:

1. Check for menu items:
   - `Window > Solis Games > SDK Settings`
   - `Window > Solis Games > Build & Deploy`

2. Check the Package Manager:
   - Go to `Window > Package Manager`
   - Find "Solis Games SDK" in the list
   - Version should show as `1.0.0` or higher

3. Check for namespace:
   - Create a new C# script
   - Add `using SolisGames;` at the top
   - If no errors, the SDK is installed correctly

---

## Post-Installation Setup

After installing, configure your API key:

1. Go to `Window > Solis Games > SDK Settings`
2. Enter your API key from the [Studio Dashboard](https://solisgames.com/studio)
3. (Optional) Enter your Game ID
4. Enable/disable features as needed
5. Click `Save Settings`

You're now ready to use the SDK! Continue to the [Quick Start Guide](QuickStart.md) to integrate the SDK into your game.

---

## Troubleshooting

### Package Manager shows "No 'git' executable was found"

**Solution:** Install Git for your operating system:
- Windows: https://git-scm.com/download/win
- macOS: Install via Homebrew (`brew install git`) or Xcode Command Line Tools
- Linux: `sudo apt-get install git` or equivalent

After installing Git, restart Unity.

### Import errors or missing files

**Solution:**
1. Remove the package: `Window > Package Manager`, find Solis Games SDK, click `Remove`
2. Delete any remaining files in `Assets/SolisGamesSDK` (if using .unitypackage method)
3. Restart Unity
4. Re-import using your preferred method

### Can't find SDK Settings menu

**Solution:**
1. Ensure the SDK was imported correctly
2. Check `Window > Package Manager` to verify the package is installed
3. Try restarting Unity
4. If still missing, try Method 3 (.unitypackage) instead

---

## Uninstalling

### If installed via Package Manager:
1. Go to `Window > Package Manager`
2. Find "Solis Games SDK"
3. Click `Remove`

### If installed via .unitypackage:
1. Delete the `Assets/SolisGamesSDK` folder
2. Delete the `Assets/Plugins/WebGL/SolisGamesSDK.jslib` file

---

## Next Steps

- [Quick Start Guide](QuickStart.md) - Get up and running in 5 minutes
- [API Reference](API-Reference.md) - Complete API documentation
- [Examples](Examples.md) - Code examples for every feature

---

## Need Help?

- 📚 [Documentation](https://solisgames.com/docs/unity)
- 💬 [GitHub Issues](https://github.com/solis-interactive/unity-sdk/issues)
- 📧 Email: support@solisgames.com
