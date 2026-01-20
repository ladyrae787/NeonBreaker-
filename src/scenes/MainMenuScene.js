// Main Menu Scene
class MainMenuScene extends Phaser.Scene {
    constructor() {
        super({ key: 'MainMenuScene' });
    }

    create() {
        const { width, height } = this.cameras.main;
        const storageManager = this.registry.get('storageManager');
        const soundManager = this.registry.get('soundManager');
        const adManager = this.registry.get('adManager');

        // Background
        this.add.rectangle(width / 2, height / 2, width, height, GameConfig.colors.background);

        // Title
        const title = this.add.text(width / 2, 200, 'BRICK\nBREAKER', {
            font: 'bold 72px Arial',
            fill: '#ffffff',
            align: 'center',
            stroke: '#4DB8FF',
            strokeThickness: 4
        });
        title.setOrigin(0.5);

        // High Score
        const highScore = storageManager.getHighScore();
        const highScoreText = this.add.text(width / 2, 350, `High Score: ${highScore}`, {
            font: '24px Arial',
            fill: '#ffffff'
        });
        highScoreText.setOrigin(0.5);

        // Play Button
        const playButton = this.createButton(width / 2, 500, 'PLAY', () => {
            soundManager.playButtonClick();
            this.scene.start('GameScene');
        });

        // Continue Button (if game in progress)
        const currentLevel = storageManager.getCurrentLevel();
        const currentScore = storageManager.getCurrentScore();
        if (currentLevel > 1 || currentScore > 0) {
            const continueButton = this.createButton(width / 2, 600, 'CONTINUE', () => {
                soundManager.playButtonClick();
                this.scene.start('GameScene', { continue: true });
            });
        }

        // Leaderboard Button
        const leaderboardButton = this.createButton(width / 2, 700, 'LEADERBOARD', () => {
            soundManager.playButtonClick();
            this.showLeaderboard();
        });

        // Settings Button (small)
        const settingsButton = this.createButton(width / 2, 800, 'SETTINGS', () => {
            soundManager.playButtonClick();
            this.showSettings();
        }, 200, 50);

        // Version
        this.add.text(width / 2, height - 50, 'v1.0.0 - HTML5', {
            font: '14px Arial',
            fill: '#666666'
        }).setOrigin(0.5);

        // Show banner ad
        if (adManager) {
            adManager.showBanner();
            adManager.loadInterstitial();
            adManager.loadRewarded();
        }
    }

    createButton(x, y, text, callback, width = 300, height = 70) {
        const button = this.add.container(x, y);

        const bg = this.add.rectangle(0, 0, width, height, GameConfig.colors.button);
        bg.setStrokeStyle(3, 0xffffff);

        const label = this.add.text(0, 0, text, {
            font: 'bold 28px Arial',
            fill: '#ffffff'
        });
        label.setOrigin(0.5);

        button.add([bg, label]);
        button.setSize(width, height);
        button.setInteractive(new Phaser.Geom.Rectangle(-width/2, -height/2, width, height), Phaser.Geom.Rectangle.Contains);

        button.on('pointerover', () => {
            bg.setFillStyle(GameConfig.colors.buttonHover);
        });

        button.on('pointerout', () => {
            bg.setFillStyle(GameConfig.colors.button);
        });

        button.on('pointerdown', callback);

        return button;
    }

    showLeaderboard() {
        const { width, height } = this.cameras.main;
        const storageManager = this.registry.get('storageManager');
        const leaderboard = storageManager.getLeaderboard();

        // Dark overlay
        const overlay = this.add.rectangle(width / 2, height / 2, width, height, 0x000000, 0.8);
        overlay.setInteractive();

        // Panel
        const panel = this.add.rectangle(width / 2, height / 2, width - 80, height - 200, 0x2a2a36);
        panel.setStrokeStyle(3, 0x4DB8FF);

        // Title
        const title = this.add.text(width / 2, 200, '🏆 LEADERBOARD', {
            font: 'bold 36px Arial',
            fill: '#ffffff'
        });
        title.setOrigin(0.5);

        // Leaderboard entries
        let yPos = 300;
        if (leaderboard.length === 0) {
            const emptyText = this.add.text(width / 2, height / 2, 'No scores yet!\nPlay to set a record.', {
                font: '24px Arial',
                fill: '#888888',
                align: 'center'
            });
            emptyText.setOrigin(0.5);
            panel.add(emptyText);
        } else {
            leaderboard.forEach((entry, index) => {
                const rank = index + 1;
                const medal = rank === 1 ? '🥇' : rank === 2 ? '🥈' : rank === 3 ? '🥉' : `${rank}.`;

                const entryText = this.add.text(width / 2, yPos,
                    `${medal} ${entry.name} - ${entry.score} (Lvl ${entry.level})`, {
                    font: '20px Arial',
                    fill: rank <= 3 ? '#FFD700' : '#ffffff'
                });
                entryText.setOrigin(0.5);
                yPos += 50;
            });
        }

        // Close button
        const closeButton = this.createButton(width / 2, height - 150, 'CLOSE', () => {
            this.registry.get('soundManager').playButtonClick();
            overlay.destroy();
            panel.destroy();
            title.destroy();
            closeButton.destroy();
        });
    }

    showSettings() {
        const { width, height } = this.cameras.main;
        const storageManager = this.registry.get('storageManager');
        const soundManager = this.registry.get('soundManager');

        // Dark overlay
        const overlay = this.add.rectangle(width / 2, height / 2, width, height, 0x000000, 0.8);
        overlay.setInteractive();

        // Panel
        const panel = this.add.rectangle(width / 2, height / 2, width - 80, 600, 0x2a2a36);
        panel.setStrokeStyle(3, 0x4DB8FF);

        // Title
        const title = this.add.text(width / 2, height / 2 - 250, '⚙️ SETTINGS', {
            font: 'bold 36px Arial',
            fill: '#ffffff'
        });
        title.setOrigin(0.5);

        // Sound toggle
        const soundText = this.add.text(width / 2 - 120, height / 2 - 150,
            `Sound: ${soundManager.isEnabled() ? 'ON' : 'OFF'}`, {
            font: '24px Arial',
            fill: '#ffffff'
        });

        const soundToggle = this.createButton(width / 2 + 100, height / 2 - 150, 'TOGGLE', () => {
            const newState = !soundManager.isEnabled();
            soundManager.setEnabled(newState);
            storageManager.setSetting('soundEnabled', newState);
            soundText.setText(`Sound: ${newState ? 'ON' : 'OFF'}`);
            soundManager.playButtonClick();
        }, 150, 50);

        // Haptics toggle
        const hapticsText = this.add.text(width / 2 - 120, height / 2 - 50,
            `Haptics: ${soundManager.isHapticsEnabled() ? 'ON' : 'OFF'}`, {
            font: '24px Arial',
            fill: '#ffffff'
        });

        const hapticsToggle = this.createButton(width / 2 + 100, height / 2 - 50, 'TOGGLE', () => {
            const newState = !soundManager.isHapticsEnabled();
            soundManager.setHapticsEnabled(newState);
            storageManager.setSetting('hapticsEnabled', newState);
            hapticsText.setText(`Haptics: ${newState ? 'ON' : 'OFF'}`);
            soundManager.playButtonClick();
        }, 150, 50);

        // Reset progress button
        const resetButton = this.createButton(width / 2, height / 2 + 80, 'RESET PROGRESS', () => {
            if (confirm('Reset all progress? This cannot be undone.')) {
                storageManager.resetProgress();
                soundManager.playButtonClick();
                this.scene.restart();
            }
        }, 280, 50);

        // Close button
        const closeButton = this.createButton(width / 2, height / 2 + 200, 'CLOSE', () => {
            soundManager.playButtonClick();
            overlay.destroy();
            panel.destroy();
            title.destroy();
            soundText.destroy();
            soundToggle.destroy();
            hapticsText.destroy();
            hapticsToggle.destroy();
            resetButton.destroy();
            closeButton.destroy();
        });
    }
}
