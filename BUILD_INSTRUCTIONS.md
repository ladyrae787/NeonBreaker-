# Build Instructions for Brick Breaker Android Game

## Quick Start Guide

### Prerequisites Check
- [ ] Unity 2021.3.0f1 installed
- [ ] Android Build Support module installed in Unity
- [ ] Android SDK/NDK configured
- [ ] JDK 8 or 11 installed

### Step 1: Open Project
```bash
# Open Unity Hub
# Click "Add" > Select this project folder
# Click on project to open in Unity Editor
```

### Step 2: Install Required Packages
1. Open Package Manager (Window > Package Manager)
2. Verify these packages are installed:
   - Unity Ads 4.4.2
   - Unity Purchasing 4.9.3
   - TextMeshPro 3.0.6
   - Unity UI 1.0.0

If any are missing, install them from the Unity Registry.

### Step 3: Create Scene Setup

#### A. Create Prefabs First

**Ball Prefab:**
1. GameObject > 2D Object > Sprites > Circle
2. Rename to "Ball"
3. Add components:
   - Rigidbody2D (Gravity Scale: 0, Collision Detection: Continuous)
   - CircleCollider2D (Radius: 0.15)
   - Ball script (from Assets/Scripts)
   - TrailRenderer (optional, for visual trail)
4. Set sprite color (white or light blue)
5. Set scale to (0.3, 0.3, 1)
6. Tag as "Ball"
7. Drag to Assets/Prefabs folder to create prefab
8. Delete from scene

**Brick Prefab:**
1. GameObject > 2D Object > Sprites > Square
2. Rename to "Brick"
3. Add components:
   - BoxCollider2D
   - Brick script (from Assets/Scripts)
4. Add child GameObject:
   - GameObject > UI > Text
   - Rename to "HealthText"
   - Center it on brick
   - Set font size: 24
   - Set alignment: center/middle
5. Link HealthText to Brick script's healthText field
6. Set brick scale to (1.2, 0.6, 1)
7. Tag as "Brick"
8. Drag to Assets/Prefabs folder
9. Delete from scene

#### B. Setup Main Scene

**Open MainGame scene:**
- Assets/Scenes/MainGame.unity

**Create Game Managers:**
1. Create empty GameObject, name "GameManager"
   - Add GameManager script
   - Position: (0, 0, 0)

2. Create empty GameObject, name "BrickManager"
   - Add BrickManager script
   - Assign Brick prefab to brickPrefab field
   - Set columns: 7
   - Set grid start position: (-4, 4, 0)
   - Position: (0, 0, 0)

3. Create empty GameObject, name "Slingshot"
   - Add SlingshotController script
   - Assign Ball prefab to ballPrefab field
   - Create child empty GameObject, name "LaunchPoint"
     - Position: (0, -4, 0)
   - Assign LaunchPoint to launchPoint field
   - Add LineRenderer component
   - Assign LineRenderer to trajectoryLine field

4. Create empty GameObject, name "AdManager"
   - Add AdManager script
   - **IMPORTANT**: Enter your Unity Ads Game ID
   - Set test mode to true for development

5. Create empty GameObject, name "IAPManager"
   - Add IAPManager script

6. Create empty GameObject, name "SoundManager"
   - Add SoundManager script
   - Add 2 AudioSource components
   - Assign one to sfxSource, one to musicSource

7. Create empty GameObject, name "UIManager"
   - Add UIManager script

8. Create empty GameObject, name "LeaderboardManager"
   - Add LeaderboardManager script

**Create Game Boundaries:**
1. Create empty GameObject, name "Walls"
2. Add child objects:
   - Left Wall:
     - GameObject > 2D Object > Sprites > Square
     - Position: (-5, 0, 0)
     - Scale: (0.2, 12, 1)
     - Add BoxCollider2D
     - Tag: "Wall"
   - Right Wall:
     - Position: (5, 0, 0)
     - Scale: (0.2, 12, 1)
     - Add BoxCollider2D
     - Tag: "Wall"
   - Top Wall:
     - Position: (0, 6, 0)
     - Scale: (12, 0.2, 1)
     - Add BoxCollider2D
     - Tag: "Wall"
   - Ground (trigger):
     - Position: (0, -5, 0)
     - Scale: (12, 0.2, 1)
     - Add BoxCollider2D
     - Set "Is Trigger" to true
     - Tag: "Ground"

**Create UI Canvas:**
1. GameObject > UI > Canvas
   - Canvas Scaler: Scale with Screen Size
   - Reference Resolution: 1080 x 1920
   - Match: 0.5

2. Add child panels (GameObject > UI > Panel):

   **Main Menu Panel:**
   - Add child: Text - "Brick Breaker" (title)
   - Add child: Button - "Play"
   - Add child: Button - "Leaderboard"
   - Add child: Button - "Settings"
   - Add child: Button - "Shop"

   **Gameplay Panel:**
   - Add child: Text - "Score: 0" (top left)
   - Add child: Text - "Level: 1" (top center)
   - Add child: Text - "Balls: 1/1" (top right)
   - Add child: Button - "Pause" (top right corner)

   **Game Over Panel:**
   - Add child: Text - "Game Over"
   - Add child: Text - "Final Score: 0"
   - Add child: Text - "New High Score!" (initially hidden)
   - Add child: Button - "Restart"
   - Add child: Button - "Watch Ad to Continue"
   - Add child: Button - "Main Menu"
   - Set active: false (initially hidden)

   **Level Complete Panel:**
   - Add child: Text - "Level Complete!"
   - Add child: Text - "Score: 0"
   - Set active: false

   **Settings Panel:**
   - Add child: Toggle - "Sound"
   - Add child: Toggle - "Music"
   - Add child: Toggle - "Haptics"
   - Add child: Slider - "SFX Volume"
   - Add child: Slider - "Music Volume"
   - Add child: Button - "Close"
   - Set active: false

   **Shop Panel:**
   - Add child: Text - "Shop"
   - Add child: Button - "Remove Ads - $2.99"
   - Add child: Button - "5 Balls - $0.99"
   - Add child: Button - "15 Balls - $1.99"
   - Add child: Button - "50 Balls - $4.99"
   - Add child: Button - "Close"
   - Add child: Text - "Status" (for purchase feedback)
   - Set active: false

   **Leaderboard Panel:**
   - Add child: Text - "Leaderboard"
   - Add child: ScrollView
     - In Content: Add Vertical Layout Group
   - Add child: Button - "Close"
   - Create Leaderboard Entry Prefab:
     - Panel with 4 Text elements: Rank, Name, Score, Level
     - Save as prefab in Assets/Prefabs
   - Set active: false

3. **Link UI to Scripts:**
   - Open UIManager in Inspector
   - Drag each panel to corresponding field
   - Drag text fields to corresponding fields
   - Drag buttons to corresponding fields
   - Open GameManager and link UI references
   - Open AdManager and link rewarded ad button
   - Open IAPManager and link shop buttons

**Link Script References:**
1. GameManager:
   - Assign all UI text fields
   - Assign BrickManager, SlingshotController, AdManager

2. BrickManager:
   - Assign brick prefab

3. SlingshotController:
   - Assign ball prefab
   - Assign launch point transform
   - Assign trajectory line renderer

4. Save scene (Ctrl+S)

### Step 4: Configure Unity Ads

1. Window > General > Services
2. Link project or create new Unity Project ID
3. Enable Unity Ads
4. Copy your Game ID
5. Open Assets/Scripts/AdManager.cs in code editor
6. Replace `YOUR_ANDROID_GAME_ID` with your actual ID
7. Save file

### Step 5: Configure Android Build Settings

1. File > Build Settings
2. Select Android
3. Click "Switch Platform" (wait for completion)
4. Click "Player Settings"

**Player Settings Configuration:**

**Company & Product:**
- Company Name: YourStudioName
- Product Name: Brick Breaker
- Version: 1.0.0
- Default Icon: (assign your 512x512 icon)

**Other Settings:**
- Package Name: com.yourstudio.brickbreaker (MUST BE UNIQUE!)
- Version: 1.0
- Bundle Version Code: 1
- Minimum API Level: Android 5.1 (API level 22)
- Target API Level: API level 33
- Install Location: Automatic
- Scripting Backend: IL2CPP
- API Compatibility Level: .NET Standard 2.1
- Target Architectures: ✓ ARM64 (required for Play Store)

**Publishing Settings:**
- Create New Keystore:
  - Click "Keystore Manager"
  - "Keystore..." > "Create New"
  - Browse: save somewhere safe (e.g., Desktop/BrickBreaker.keystore)
  - Password: (choose strong password - SAVE THIS!)
  - Alias: brick_breaker_key
  - Password: (same or different - SAVE THIS!)
  - Common Name: Your Name
  - Organizational Unit: Your Studio
  - Organization: Your Studio
  - City, State, Country: Your location
  - Click "Add Key"
- **IMPORTANT**: Back up this keystore file! You cannot update your app without it.

### Step 6: Test in Unity Editor

1. Click Play button in Unity Editor
2. Verify:
   - [ ] Main menu appears
   - [ ] Can start game
   - [ ] Bricks spawn correctly
   - [ ] Can drag to aim
   - [ ] Balls launch when releasing drag
   - [ ] Balls collide with bricks
   - [ ] Bricks take damage and destroy
   - [ ] Score updates
   - [ ] Game over triggers correctly
   - [ ] UI panels work

### Step 7: Build Development APK

1. File > Build Settings
2. Ensure scenes are added:
   - ✓ Scenes/MainGame
3. Click "Build"
4. Choose save location
5. Name: BrickBreaker_v1.0_dev.apk
6. Wait for build (5-10 minutes first time)

### Step 8: Test on Android Device

**Install via USB:**
```bash
# Enable Developer Options on Android device
# Enable USB Debugging
# Connect device via USB

# Install APK
adb install BrickBreaker_v1.0_dev.apk

# Check logs
adb logcat -s Unity
```

**Test checklist:**
- [ ] App launches
- [ ] Touch controls work
- [ ] Gameplay is smooth (target 60 FPS)
- [ ] Ads show (in test mode)
- [ ] Sound plays
- [ ] Haptic feedback works
- [ ] No crashes

### Step 9: Build Release AAB for Play Store

**Pre-release checklist:**
- [ ] Tested on multiple devices
- [ ] All bugs fixed
- [ ] Ads configured (test mode OFF)
- [ ] IAP products configured
- [ ] Privacy policy added
- [ ] Keystore backed up safely
- [ ] Version code incremented

**Build process:**
1. Open AdManager.cs
2. Change `testMode = false`
3. Save all changes
4. File > Build Settings > Android
5. Build App Bundle (AAB): ✓ ON
6. Click "Build"
7. Save as: BrickBreaker_v1.0.aab
8. Wait for build to complete

### Step 10: Upload to Google Play Console

1. Go to play.google.com/console
2. Create new app or select existing
3. Fill in store listing:
   - Title: Brick Breaker
   - Short description
   - Full description
   - Screenshots (at least 2)
   - Feature graphic (1024x500)
   - App icon (512x512)
   - Privacy policy URL
4. Content rating questionnaire
5. Target audience and content
6. Release > Production > Create release
7. Upload BrickBreaker_v1.0.aab
8. Release notes
9. Review and rollout

## Troubleshooting

### "Package Name already exists"
Solution: Change package name in Player Settings to something unique.

### "Unable to find Unity Ads"
Solution: Package Manager > Unity Registry > Install Unity Ads

### "Gradle build failed"
Solution:
- Unity > Preferences > External Tools
- Check Android SDK, NDK, JDK paths
- Try: Edit > Preferences > External Tools > "Reset to default"

### "Keystore password incorrect"
Solution: Recreate keystore (for development builds only!)

### Ads not showing on device
Solution:
- Check internet connection
- Verify Game ID in AdManager.cs
- Wait 24-48 hours for ad inventory
- Use test mode during development

### App crashes on launch
Solution:
- Check logcat: `adb logcat -s Unity`
- Verify all prefab references in scenes
- Check for null reference exceptions
- Test with development build for better logs

## Performance Optimization

If game is laggy:
1. Reduce ball count
2. Lower physics timestep (Edit > Project Settings > Time)
3. Disable fancy effects
4. Use simpler collision shapes
5. Reduce UI overdraw
6. Target 60 FPS in profiler

## Success Criteria

Before publishing:
- [ ] No crashes
- [ ] Smooth gameplay (60 FPS)
- [ ] Ads display correctly
- [ ] IAP purchases work
- [ ] Leaderboard saves
- [ ] Sound works
- [ ] All UI functional
- [ ] Game over/restart works
- [ ] Level progression works
- [ ] Tested on 3+ devices
- [ ] APK size under 150 MB
- [ ] Privacy policy added
- [ ] GDPR compliant (if EU)

## Next Steps After Publishing

1. Monitor crash reports in Play Console
2. Respond to user reviews
3. Plan updates and new features
4. A/B test ad placements
5. Optimize monetization
6. Add analytics
7. Marketing and user acquisition

Good luck! 🎮
