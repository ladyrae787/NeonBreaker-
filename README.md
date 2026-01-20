# Brick Breaker - HTML5 Mobile Game

A modern brick breaker game built with HTML5/JavaScript and Phaser.js, optimized for mobile touchscreens. **Can be developed entirely from your phone!**

## Features

- **Slingshot Touch Controls**: Intuitive drag-to-aim mechanic with visual trajectory
- **Progressive Difficulty**: Brick health increases with level and row position
- **Cumulative Ball System**: Collect balls that persist and grow with each level
- **Scoring & Leaderboard**: Points system with local high score tracking (top 10)
- **Full Monetization**:
  - AdMob integration (banner, interstitial, rewarded video)
  - In-app purchases ready (via Capacitor)
- **Minimalist Design**: Clean, flat UI optimized for mobile
- **Sound & Haptics**: Web Audio API tones + Vibration API
- **Offline Play**: Works without internet (ads require connection)
- **Responsive**: Adapts to any screen size

## 🚀 Quick Start (Test in Browser)

### Option 1: Simple Python Server (No Installation)

```bash
# From your phone terminal or GitHub Codespaces
python3 -m http.server 8080
```

Then open in browser: `http://localhost:8080`

### Option 2: Live Preview (GitHub Codespaces)

1. Open repository in GitHub Codespaces
2. Right-click `index.html` → "Open with Live Server"
3. Game opens in browser automatically

### Option 3: Direct File Open

Simply open `index.html` in any modern web browser!

## 📱 Development from Your Phone

### Using GitHub Codespaces (Recommended)

1. **Open repository on GitHub** (on your phone)
2. Tap the **Code** button → **Codespaces** tab
3. Tap **"Create codespace on main"**
4. Wait for environment to load (~30 seconds)
5. **Edit any file** directly in the browser!
6. **Test changes**: Run `python3 -m http.server 8080` in terminal
7. Open preview in browser

**Files you can edit:**
- `src/config.js` - Game settings (ball speed, colors, difficulty)
- `src/scenes/GameScene.js` - Core gameplay mechanics
- `src/scenes/MainMenuScene.js` - Menu and UI
- `index.html` - Layout and styling

### Using Mobile Code Editors

**Spck Editor** (Android):
- Install from Play Store (free)
- Clone this repository
- Edit JavaScript files
- Run local server to test

**Code Editor** (iOS):
- Install from App Store
- Clone repository via Git
- Edit files
- Preview changes

## 🎮 How to Play

1. **Aim**: Drag from the launch point to aim
2. **Release**: Let go to fire all balls
3. **Break Bricks**: Each brick shows its health number
4. **Collect Balls**: Complete levels to earn more balls
5. **Progress**: Clear all bricks to advance

## 🛠️ Project Structure

```
brick-breaker/
├── index.html                  # Main HTML file
├── package.json                # npm dependencies
├── capacitor.config.json       # Android build config
├── src/
│   ├── config.js               # Game configuration
│   ├── game.js                 # Phaser initialization
│   ├── managers/
│   │   ├── AdManager.js        # AdMob integration
│   │   ├── SoundManager.js     # Audio & haptics
│   │   └── StorageManager.js   # LocalStorage (saves/leaderboard)
│   └── scenes/
│       ├── BootScene.js        # Loading & initialization
│       ├── MainMenuScene.js    # Main menu
│       ├── GameScene.js        # Core gameplay
│       └── GameOverScene.js    # Game over screen
├── assets/                     # (Optional) Images & sounds
├── ralph-wiggum/               # AI development loop plugin
└── README.md                   # This file
```

## 📦 Building for Android

### Prerequisites

- Node.js 16+ (on desktop or cloud)
- Android Studio (for final build)
- OR use GitHub Actions (automated)

### Method 1: Manual Build (Desktop Required)

```bash
# Install dependencies
npm install

# Install Capacitor
npm install @capacitor/core @capacitor/cli @capacitor/android

# Initialize Capacitor
npx cap init

# Add Android platform
npx cap add android

# Sync web files to Android
npx cap sync

# Open in Android Studio
npx cap open android

# In Android Studio:
# - Build → Build Bundle(s) / APK(s) → Build APK(s)
# - Or Build → Generate Signed Bundle for Play Store
```

### Method 2: Cloud Build (No Desktop Needed!)

**Using GitHub Actions:**

1. Fork this repository
2. Enable GitHub Actions
3. Push changes to trigger build
4. Download APK from Actions artifacts

*(GitHub Actions workflow file coming soon)*

**Using Capacitor Cloud:**

```bash
npx @capacitor/cli cloud build android
```

Builds in the cloud, downloads APK to your device!

## 🎨 Customization

### Change Colors

Edit `src/config.js`:

```javascript
brick: {
    colors: [
        0x4DB8FF, // Light Blue
        0x66CC66, // Green - Change these!
        0xFFCC33, // Yellow
        // ... add more colors
    ]
}
```

### Adjust Difficulty

```javascript
ball: {
    minSpeed: 400,  // Lower = slower
    maxSpeed: 600,  // Lower = easier to control
}
```

### Change Scoring

```javascript
scoring: {
    baseScore: 10  // Higher = more points per brick
}
```

## 💰 Monetization Setup

### AdMob Integration

1. Create AdMob account: [admob.google.com](https://admob.google.com)
2. Create new app
3. Copy ad unit IDs
4. Replace in `src/config.js`:

```javascript
ads: {
    bannerId: 'ca-app-pub-XXXXXXXXXXXXXXXX/YYYYYYYYYY',
    interstitialId: 'ca-app-pub-XXXXXXXXXXXXXXXX/ZZZZZZZZZZ',
    rewardedId: 'ca-app-pub-XXXXXXXXXXXXXXXX/WWWWWWWWWW',
    testMode: false  // Set to false for production!
}
```

### In-App Purchases

Uses Capacitor's In-App Purchase plugin. Configure in Google Play Console after uploading APK.

## 🧪 Testing

### Test in Browser

```bash
python3 -m http.server 8080
# or
npm run serve
```

Open `http://localhost:8080` in Chrome DevTools mobile emulator.

### Test on Android Device

1. Build APK
2. Install via ADB:
   ```bash
   adb install path/to/app-debug.apk
   ```
3. Or transfer APK to phone and install

## 📋 Publishing to Google Play Store

1. **Build signed AAB**:
   - Android Studio → Build → Generate Signed Bundle
   - Create keystore (save it securely!)
   - Build release AAB

2. **Prepare store listing**:
   - App name: Brick Breaker
   - Category: Games > Arcade
   - Content rating: ESRB Everyone
   - Screenshots (at least 2)
   - Feature graphic (1024x500)
   - App icon (512x512)

3. **Upload to Play Console**:
   - Create new app
   - Upload AAB to internal testing first
   - Test thoroughly
   - Submit for review
   - Rollout to production

## 🐛 Troubleshooting

### Game not loading in browser

- Check browser console for errors (F12)
- Ensure Phaser CDN is accessible
- Try hard refresh (Ctrl+Shift+R)

### Touch controls not working

- Ensure `touch-action: none` in CSS
- Test on actual device, not just desktop emulator
- Check if pointer events are enabled

### Ads not showing

- Test mode: Uses test ad IDs (always show)
- Production: Wait 24-48 hours for ad inventory
- Verify AdMob account is active
- Check internet connection

### APK build fails

- Ensure Android SDK is installed
- Check `capacitor.config.json` is valid
- Run `npx cap sync` before building
- Check Android Studio logs

## 🎯 Performance Tips

- Game targets 60 FPS
- Uses hardware acceleration
- Minimal DOM manipulation
- Procedural graphics (no image files needed)

## 📜 License

MIT License - Feel free to use for commercial projects!

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Make changes
4. Test on multiple devices
5. Submit pull request

## 🎓 Learning Resources

- **Phaser 3**: [phaser.io/learn](https://phaser.io/learn)
- **Capacitor**: [capacitorjs.com/docs](https://capacitorjs.com/docs)
- **AdMob**: [admob.google.com/support](https://admob.google.com/support)
- **Web Audio API**: [developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API)

## 📞 Support

- **Report bugs**: Open GitHub issue
- **Questions**: Check Discussions tab
- **Updates**: Watch repository for releases

## 🚀 Next Steps

1. **Test the game**: Open `index.html` in browser
2. **Customize**: Edit `src/config.js` with your preferences
3. **Add AdMob**: Replace test IDs with real ones
4. **Build APK**: Follow build instructions above
5. **Publish**: Upload to Google Play Store
6. **Profit**: Watch the downloads roll in! 💰

---

**Made with ❤️ using HTML5, Phaser.js, and Capacitor**

**Star ⭐ this repo if you found it helpful!**
