# Quick Start Sample

The simplest possible integration of the Solis Games SDK.

## What This Example Shows

- SDK initialization
- Getting user data
- Submitting scores to leaderboards
- Tracking analytics events
- Saving data to cloud

## Setup Instructions

1. **Import this sample:**
   - Window > Package Manager > Solis Games SDK > Samples > Quick Start > Import

2. **Configure your API key:**
   - Open the QuickStart scene
   - Select the "SolisSDK" GameObject in the hierarchy
   - Enter your API key in the Inspector (get one from solisgames.com/studio)

3. **Run the example:**
   - Press Play in the Unity Editor
   - Check the Console for output

## Expected Output

```
=== Solis Games SDK - Quick Start Example ===
Initializing SDK...
✅ SDK initialized successfully!
User Data:
  Username: DemoPlayer
  Email: demo@example.com
  Premium: False
Score Submitted:
  Score: 742
  Rank: #42
Analytics event tracked: quick_start_complete
Game data saved to cloud!
=== Quick Start Example Complete! ===
```

## Next Steps

- Check out the **FullFeatures** sample for advanced usage
- Read the [API Reference](../../Documentation~/API-Reference.md) for complete documentation
- Join our Discord: https://discord.gg/TZdBFBhW

## Files

- `QuickStartExample.cs` - Main example script (attach to a GameObject)
- `README.md` - This file

## Need Help?

- Documentation: https://solisgames.com/docs/unity
- Discord: https://discord.gg/TZdBFBhW
- Support: support@solisgames.com
