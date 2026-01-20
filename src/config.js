// Game Configuration
const GameConfig = {
    // Game dimensions
    width: 720,
    height: 1280,

    // Physics
    gravity: 0,

    // Ball settings
    ball: {
        radius: 8,
        minSpeed: 400,
        maxSpeed: 600,
        launchDelay: 100
    },

    // Brick settings
    brick: {
        width: 90,
        height: 40,
        spacing: 5,
        columns: 7,
        colors: [
            0x4DB8FF, // Light Blue
            0x66CC66, // Green
            0xFFCC33, // Yellow
            0xFF9933, // Orange
            0xFF6666, // Red
            0xCC66CC  // Purple
        ]
    },

    // Scoring
    scoring: {
        baseScore: 10
    },

    // UI colors
    colors: {
        background: 0x1a1a26,
        text: 0xffffff,
        button: 0x4DB8FF,
        buttonHover: 0x66CCFF
    },

    // Ads configuration (AdMob for mobile)
    ads: {
        enabled: true,
        testMode: true,
        bannerId: 'ca-app-pub-3940256099942544/6300978111', // Test ID
        interstitialId: 'ca-app-pub-3940256099942544/1033173712', // Test ID
        rewardedId: 'ca-app-pub-3940256099942544/5224354917', // Test ID
        interstitialFrequency: 3 // Show every 3 levels
    },

    // Sound settings
    audio: {
        enabled: true,
        musicVolume: 0.3,
        sfxVolume: 0.7
    }
};
