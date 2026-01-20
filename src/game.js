// Main Game Initialization
const config = {
    type: Phaser.AUTO,
    width: GameConfig.width,
    height: GameConfig.height,
    parent: 'game-container',
    backgroundColor: GameConfig.colors.background,
    scale: {
        mode: Phaser.Scale.FIT,
        autoCenter: Phaser.Scale.CENTER_BOTH,
        width: GameConfig.width,
        height: GameConfig.height
    },
    physics: {
        default: 'arcade',
        arcade: {
            gravity: { y: 0 },
            debug: false
        }
    },
    input: {
        activePointers: 3
    },
    scene: [
        BootScene,
        MainMenuScene,
        GameScene,
        GameOverScene
    ]
};

// Create game instance
const game = new Phaser.Game(config);

// Prevent context menu on long press
document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

// Handle page visibility (pause when hidden)
document.addEventListener('visibilitychange', function() {
    if (document.hidden) {
        game.scene.pause();
    } else {
        game.scene.resume();
    }
});
