// Game Over Scene
class GameOverScene extends Phaser.Scene {
    constructor() {
        super({ key: 'GameOverScene' });
    }

    init(data) {
        this.finalScore = data.score || 0;
        this.finalLevel = data.level || 1;
        this.isNewHigh = data.isNewHigh || false;
    }

    create() {
        const { width, height } = this.cameras.main;
        const soundManager = this.registry.get('soundManager');
        const adManager = this.registry.get('adManager');
        const storageManager = this.registry.get('storageManager');

        // Background
        this.add.rectangle(width / 2, height / 2, width, height, GameConfig.colors.background);

        // Game Over title
        const titleText = this.add.text(width / 2, 200, 'GAME OVER', {
            font: 'bold 64px Arial',
            fill: '#ff6666',
            stroke: '#ffffff',
            strokeThickness: 4
        });
        titleText.setOrigin(0.5);

        // Score
        const scoreText = this.add.text(width / 2, 320, `Score: ${this.finalScore}`, {
            font: 'bold 36px Arial',
            fill: '#ffffff'
        });
        scoreText.setOrigin(0.5);

        // Level reached
        const levelText = this.add.text(width / 2, 380, `Level Reached: ${this.finalLevel}`, {
            font: '28px Arial',
            fill: '#ffffff'
        });
        levelText.setOrigin(0.5);

        // New high score
        if (this.isNewHigh) {
            const newHighText = this.add.text(width / 2, 450, '🏆 NEW HIGH SCORE! 🏆', {
                font: 'bold 32px Arial',
                fill: '#FFD700'
            });
            newHighText.setOrigin(0.5);

            // Animate
            this.tweens.add({
                targets: newHighText,
                scaleX: 1.1,
                scaleY: 1.1,
                duration: 500,
                yoyo: true,
                repeat: -1
            });
        }

        // Buttons
        let buttonY = this.isNewHigh ? 550 : 500;

        // Restart button
        const restartButton = this.createButton(width / 2, buttonY, 'PLAY AGAIN', () => {
            soundManager.playButtonClick();
            this.scene.start('GameScene');
        });

        // Continue with rewarded ad button
        if (adManager.isRewardedAvailable()) {
            const continueButton = this.createButton(width / 2, buttonY + 100, 'WATCH AD TO CONTINUE', () => {
                soundManager.playButtonClick();
                adManager.showRewarded(
                    () => {
                        // Reward: restore session
                        // For now, just add extra balls
                        const currentBalls = storageManager.getTotalBalls();
                        storageManager.setTotalBalls(currentBalls + 5);

                        this.add.text(width / 2, height / 2, '+5 Balls!\nReward Granted', {
                            font: 'bold 36px Arial',
                            fill: '#4DB8FF',
                            align: 'center'
                        }).setOrigin(0.5);

                        this.time.delayedCall(2000, () => {
                            this.scene.start('MainMenuScene');
                        });
                    },
                    () => {
                        console.log('Rewarded ad dismissed');
                    }
                );
            }, 350, 70);

            buttonY += 100;
        }

        // Main menu button
        const menuButton = this.createButton(width / 2, buttonY + 100, 'MAIN MENU', () => {
            soundManager.playButtonClick();
            this.scene.start('MainMenuScene');
        });

        // High score
        const highScore = storageManager.getHighScore();
        const highScoreText = this.add.text(width / 2, height - 100, `High Score: ${highScore}`, {
            font: '24px Arial',
            fill: '#888888'
        });
        highScoreText.setOrigin(0.5);
    }

    createButton(x, y, text, callback, width = 350, height = 70) {
        const button = this.add.container(x, y);

        const bg = this.add.rectangle(0, 0, width, height, GameConfig.colors.button);
        bg.setStrokeStyle(3, 0xffffff);

        const label = this.add.text(0, 0, text, {
            font: 'bold 24px Arial',
            fill: '#ffffff',
            wordWrap: { width: width - 20 },
            align: 'center'
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
}
