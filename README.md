# Brick Breaker - Android Game

A modern brick breaker game reimagined for touch screens with slingshot-style controls. Built with Unity 2021.3.0f1 for Android.

## Features

- **Slingshot Touch Controls**: Intuitive drag-to-aim mechanic optimized for mobile
- **Progressive Difficulty**: Brick health increases with each level and row
- **Cumulative Ball System**: Collect balls that persist across rounds
- **Scoring System**: Points based on brick values and level progression
- **High Score Leaderboard**: Local persistence of top 10 scores
- **Monetization**:
  - Banner ads
  - Interstitial ads (every 3 levels)
  - Rewarded video ads (continue after game over)
  - In-app purchases (remove ads, extra balls)
- **Minimalist Design**: Clean, flat UI with smooth animations
- **Sound & Haptics**: Audio feedback and vibration for actions
- **Game Over Logic**: Continue with rewarded ads or restart

## Project Structure

```
Assets/
├── Scenes/
│   └── MainGame.unity          # Main game scene
├── Scripts/
│   ├── GameManager.cs          # Core game logic and state management
│   ├── SlingshotController.cs  # Touch controls and ball launching
│   ├── Ball.cs                 # Ball physics and collision
│   ├── Brick.cs                # Brick behavior and health
│   ├── BrickManager.cs         # Brick spawning and grid management
│   ├── AdManager.cs            # Unity Ads integration
│   ├── IAPManager.cs           # In-app purchases
│   ├── LeaderboardManager.cs   # High score tracking
│   ├── SoundManager.cs         # Audio and haptics
│   └── UIManager.cs            # UI panels and transitions
├── Prefabs/                    # Game object prefabs (to be created in Unity)
├── Materials/                  # Materials for game objects
├── UI/                         # UI sprites and assets
└── Plugins/
    └── Android/
        └── AndroidManifest.xml # Android configuration

ProjectSettings/
├── ProjectVersion.txt          # Unity version info
└── ProjectSettings.asset       # Project configuration (created in Unity Editor)

Packages/
└── manifest.json               # Package dependencies
```

## Prerequisites

- Unity 2021.3.0f1 or later
- Android SDK (API Level 21 minimum, target 33)
- JDK 8 or 11
- Unity Ads account and Game ID
- Google Play Console account (for publishing)

## Setup Instructions

### 1. Open Project in Unity

1. Open Unity Hub
2. Click "Add" and select this project folder
3. Ensure Unity version 2021.3.0f1 is installed
4. Open the project

### 2. Configure Unity Ads

1. Go to Window > General > Services
2. Create a Unity Project ID or link existing
3. Enable Unity Ads
4. Get your Game IDs for Android/iOS
5. Open `Assets/Scripts/AdManager.cs`
6. Replace `YOUR_ANDROID_GAME_ID` with your actual Game ID
7. Update ad unit IDs if using custom placements

### 3. Configure In-App Purchases

1. Create products in Google Play Console
2. Open `Assets/Scripts/IAPManager.cs`
3. Update product IDs to match your Play Console products
4. Test with Google Play's testing track

### 4. Setup Android Build

1. Go to File > Build Settings
2. Select Android platform
3. Click "Switch Platform"
4. Go to Player Settings
5. Configure:
   - **Company Name**: Your studio name
   - **Product Name**: Brick Breaker
   - **Package Name**: `com.yourstudio.brickbreaker` (must be unique)
   - **Version**: 1.0
   - **Bundle Version Code**: 1
   - **Minimum API Level**: Android 5.1 (API 22)
   - **Target API Level**: API 33
   - **Scripting Backend**: IL2CPP
   - **Target Architectures**: ARM64 (required for Play Store)

### 5. Create Game Objects in Unity Editor

Before building, you need to set up the scene in Unity Editor:

#### Required Prefabs:
1. **Ball Prefab**:
   - Create a Circle sprite with CircleCollider2D and Rigidbody2D
   - Attach `Ball.cs` script
   - Add TrailRenderer for visual effect
   - Tag as "Ball"

2. **Brick Prefab**:
   - Create a Quad sprite with BoxCollider2D
   - Attach `Brick.cs` script
   - Add Text component for health display
   - Tag as "Brick"

#### Main Scene Setup:
1. Open `Assets/Scenes/MainGame.unity`
2. Create game objects:
   - **GameManager**: Empty GameObject with GameManager.cs
   - **BrickManager**: Empty GameObject with BrickManager.cs
   - **SlingshotController**: GameObject with SlingshotController.cs
   - **AdManager**: GameObject with AdManager.cs
   - **IAPManager**: GameObject with IAPManager.cs
   - **SoundManager**: GameObject with SoundManager.cs
   - **UIManager**: GameObject with UIManager.cs
   - **LeaderboardManager**: GameObject with LeaderboardManager.cs

3. Create UI Canvas:
   - Add Canvas (Screen Space - Overlay)
   - Add UI panels for: Main Menu, Gameplay HUD, Game Over, Settings, Shop, Leaderboard
   - Link UI elements to UIManager script

4. Create Game Boundaries:
   - Add EdgeCollider2D or BoxCollider2D for walls (tag as "Wall")
   - Add ground trigger at bottom (tag as "Ground")

5. Wire up script references in Inspector

### 6. Build for Android

#### Development Build:
```bash
# From Unity Editor:
# File > Build Settings > Build
# Select output folder
# Wait for build to complete
```

#### Release Build (for Play Store):
1. Go to Player Settings > Publishing Settings
2. Create a new keystore:
   - Click "Keystore Manager"
   - "Keystore... > Create New"
   - Fill in all details and remember the passwords
3. Build Settings > Build App Bundle (AAB)
4. This creates an .aab file for Play Store upload

### 7. Testing

1. **Unity Editor**: Test basic gameplay
2. **Development Build**: Install APK on device via ADB
3. **Test Ads**: Enable test mode in AdManager
4. **Test IAP**: Use Google Play Console test tracks
5. **Performance**: Test on multiple Android devices

## Building for Google Play Store

### Pre-launch Checklist:

- [ ] Update version code in Player Settings
- [ ] Set release keystore in Publishing Settings
- [ ] Disable ad test mode in AdManager.cs
- [ ] Configure IAP product IDs
- [ ] Replace `YOUR_ANDROID_GAME_ID` with real Unity Ads ID
- [ ] Update package name if needed
- [ ] Create app icon (512x512) and feature graphic
- [ ] Test on multiple devices
- [ ] Check crash logs and fix bugs
- [ ] Verify GDPR compliance for ads
- [ ] Add privacy policy URL
- [ ] Build signed AAB (not APK)

### Build Command:
```bash
# In Unity Editor:
File > Build Settings > Android
Build Type: App Bundle (AAB)
Click "Build"
```

### Upload to Play Console:

1. Go to Google Play Console
2. Create new app or select existing
3. Navigate to Release > Production
4. Create new release
5. Upload the .aab file
6. Fill in release notes
7. Review and rollout

## Troubleshooting

### Build Errors:

**"Unable to find Unity Ads package"**
- Open Package Manager (Window > Package Manager)
- Search for "Unity Ads" and install

**"Gradle build failed"**
- Update Android SDK in Unity preferences
- Check JDK installation
- Clear Gradle cache: `~/.gradle/caches`

**"Keystore not found"**
- Recreate keystore in Player Settings > Publishing Settings
- Ensure paths don't have spaces

### Runtime Issues:

**Ads not showing**
- Verify Game ID in AdManager.cs
- Check internet connection
- Enable test mode during development
- Wait for ad inventory to fill (can take 24-48 hours)

**IAP not working**
- Verify product IDs match Play Console
- Test with license testing account
- Ensure app is published to internal/alpha track
- Check billing permissions in manifest

**Game lags on device**
- Reduce physics timestep
- Optimize collision detection
- Reduce particle effects
- Target 60 FPS with VSync off

## Customization

### Change Colors:
- Edit `brickColors` array in BrickManager.cs
- Modify camera background color in scene

### Adjust Difficulty:
- Change brick health formula in Brick.cs `Initialize()`
- Modify `maxLaunchForce` in SlingshotController.cs
- Adjust ball speed in Ball.cs

### Add Power-ups:
- Create new PowerUp.cs script
- Add PowerUpManager.cs
- Spawn power-ups in BrickManager

### Change Monetization:
- Adjust ad frequency in GameManager.cs
- Add/remove IAP products in IAPManager.cs
- Modify pricing in Play Console

## Assets Needed

To complete the game, create these assets:

### Graphics:
- App icon (512x512 PNG)
- Splash screen (1920x1080)
- Ball sprite (circular, 128x128)
- Brick sprite (rectangular, 256x128)
- UI buttons and panels
- Background gradient/texture

### Audio (optional):
- Ball launch sound
- Bounce sound
- Brick break sound
- Level complete jingle
- Game over sound
- Background music (loop)
- UI button click

## License

This project is provided as-is for educational and commercial use.

## Support

For issues or questions:
- Check Unity documentation: https://docs.unity3d.com
- Unity Ads setup: https://docs.unity.com/ads
- Google Play Console: https://support.google.com/googleplay/android-developer

## Next Steps

1. Open project in Unity 2021.3.0f1
2. Set up scene with game objects and UI
3. Create prefabs for Ball and Brick
4. Configure Unity Ads and IAP
5. Test gameplay in editor
6. Build and test on Android device
7. Optimize and polish
8. Prepare store assets
9. Build signed AAB
10. Upload to Play Store

Good luck with your game!
