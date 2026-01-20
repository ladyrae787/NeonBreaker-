// Main Game Scene - Core Gameplay
class GameScene extends Phaser.Scene {
    constructor() {
        super({ key: 'GameScene' });
    }

    init(data) {
        this.continuing = data.continue || false;
    }

    create() {
        const { width, height } = this.cameras.main;

        // Get managers
        this.storageManager = this.registry.get('storageManager');
        this.soundManager = this.registry.get('soundManager');
        this.adManager = this.registry.get('adManager');

        // Game state
        if (this.continuing) {
            this.currentLevel = this.storageManager.getCurrentLevel();
            this.score = this.storageManager.getCurrentScore();
            this.totalBalls = this.storageManager.getTotalBalls();
        } else {
            this.currentLevel = 1;
            this.score = 0;
            this.totalBalls = 1;
            this.storageManager.resetProgress();
        }

        this.ballsRemaining = this.totalBalls;
        this.activeBalls = [];
        this.bricks = [];
        this.isLaunching = false;
        this.gameOver = false;

        // Background
        this.add.rectangle(width / 2, height / 2, width, height, GameConfig.colors.background);

        // UI
        this.createUI();

        // Game boundaries
        this.createBoundaries();

        // Launch point
        this.launchPoint = { x: width / 2, y: height - 150 };
        this.launchIndicator = this.add.circle(this.launchPoint.x, this.launchPoint.y, 15, 0xffffff, 0.5);

        // Trajectory line
        this.trajectoryGraphics = this.add.graphics();

        // Input
        this.input.on('pointerdown', this.onPointerDown, this);
        this.input.on('pointermove', this.onPointerMove, this);
        this.input.on('pointerup', this.onPointerUp, this);

        this.dragStart = null;
        this.isDragging = false;

        // Spawn bricks
        this.spawnBricks();

        // Save progress
        this.saveProgress();
    }

    createUI() {
        const { width } = this.cameras.main;

        // Score
        this.scoreText = this.add.text(20, 20, `Score: ${this.score}`, {
            font: 'bold 24px Arial',
            fill: '#ffffff'
        });

        // Level
        this.levelText = this.add.text(width / 2, 20, `Level: ${this.currentLevel}`, {
            font: 'bold 24px Arial',
            fill: '#ffffff'
        });
        this.levelText.setOrigin(0.5, 0);

        // Balls
        this.ballsText = this.add.text(width - 20, 20, `Balls: ${this.ballsRemaining}/${this.totalBalls}`, {
            font: 'bold 24px Arial',
            fill: '#ffffff'
        });
        this.ballsText.setOrigin(1, 0);

        // Pause button
        const pauseBtn = this.add.text(width - 20, 60, '⏸', {
            font: '32px Arial',
            fill: '#ffffff'
        });
        pauseBtn.setOrigin(1, 0);
        pauseBtn.setInteractive();
        pauseBtn.on('pointerdown', () => {
            this.soundManager.playButtonClick();
            this.scene.pause();
            this.showPauseMenu();
        });
    }

    createBoundaries() {
        const { width, height } = this.cameras.main;

        // Walls (invisible physics bodies)
        this.physics.add.existing(
            this.add.rectangle(0, height / 2, 20, height, 0xffffff, 0).setOrigin(0, 0.5)
        ).body.setImmovable(true);

        this.physics.add.existing(
            this.add.rectangle(width, height / 2, 20, height, 0xffffff, 0).setOrigin(1, 0.5)
        ).body.setImmovable(true);

        this.physics.add.existing(
            this.add.rectangle(width / 2, 0, width, 20, 0xffffff, 0).setOrigin(0.5, 0)
        ).body.setImmovable(true);

        // Ground (trigger zone)
        this.ground = this.add.rectangle(width / 2, height + 10, width, 20, 0xff0000, 0);
        this.physics.add.existing(this.ground);
        this.ground.body.setImmovable(true);
    }

    spawnBricks() {
        const { width } = this.cameras.main;
        const startY = 150;
        const { brick } = GameConfig;

        // Calculate grid dimensions
        const totalWidth = (brick.columns * brick.width) + ((brick.columns - 1) * brick.spacing);
        const startX = (width - totalWidth) / 2;

        // Spawn rows based on level (max 10 rows)
        const rowsToSpawn = Math.min(this.currentLevel, 10);

        for (let row = 0; row < rowsToSpawn; row++) {
            for (let col = 0; col < brick.columns; col++) {
                const x = startX + (col * (brick.width + brick.spacing)) + (brick.width / 2);
                const y = startY + (row * (brick.height + brick.spacing));

                this.createBrick(x, y, row);
            }
        }
    }

    createBrick(x, y, row) {
        const { brick } = GameConfig;

        // Calculate health: baseHealth + level + row
        const health = 1 + this.currentLevel + row;

        // Container for brick
        const brickContainer = this.add.container(x, y);

        // Brick body
        const colorIndex = Math.min(row, brick.colors.length - 1);
        const body = this.add.rectangle(0, 0, brick.width, brick.height, brick.colors[colorIndex]);
        body.setStrokeStyle(2, 0xffffff);

        // Health text
        const healthText = this.add.text(0, 0, health.toString(), {
            font: 'bold 24px Arial',
            fill: '#ffffff'
        });
        healthText.setOrigin(0.5);

        brickContainer.add([body, healthText]);

        // Physics
        this.physics.add.existing(brickContainer);
        brickContainer.body.setImmovable(true);
        brickContainer.body.setSize(brick.width, brick.height);

        // Data
        brickContainer.setData('health', health);
        brickContainer.setData('maxHealth', health);
        brickContainer.setData('row', row);
        brickContainer.setData('healthText', healthText);
        brickContainer.setData('body', body);

        this.bricks.push(brickContainer);
    }

    onPointerDown(pointer) {
        if (this.isLaunching || this.gameOver || this.activeBalls.length > 0) return;

        // Allow dragging from anywhere on screen
        this.isDragging = true;
        this.dragStart = { x: pointer.x, y: pointer.y };
    }

    onPointerMove(pointer) {
        if (!this.isDragging) return;

        const dragVector = {
            x: this.dragStart.x - pointer.x,
            y: this.dragStart.y - pointer.y
        };

        // Clamp drag distance
        const maxDrag = 200;
        const distance = Math.sqrt(dragVector.x ** 2 + dragVector.y ** 2);
        if (distance > maxDrag) {
            dragVector.x = (dragVector.x / distance) * maxDrag;
            dragVector.y = (dragVector.y / distance) * maxDrag;
        }

        // Draw trajectory
        this.drawTrajectory(dragVector);
    }

    onPointerUp(pointer) {
        if (!this.isDragging) return;

        this.isDragging = false;
        this.trajectoryGraphics.clear();

        const dragVector = {
            x: this.dragStart.x - pointer.x,
            y: this.dragStart.y - pointer.y
        };

        // Launch balls
        this.launchBalls(dragVector);
    }

    drawTrajectory(dragVector) {
        this.trajectoryGraphics.clear();

        const velocity = {
            x: dragVector.x * 3,
            y: dragVector.y * 3
        };

        // Clamp velocity
        const speed = Math.sqrt(velocity.x ** 2 + velocity.y ** 2);
        if (speed > GameConfig.ball.maxSpeed) {
            velocity.x = (velocity.x / speed) * GameConfig.ball.maxSpeed;
            velocity.y = (velocity.y / speed) * GameConfig.ball.maxSpeed;
        }

        // Draw trajectory as dotted line
        let x = this.launchPoint.x;
        let y = this.launchPoint.y;

        // Draw dots along trajectory path
        for (let i = 0; i < 30; i++) {
            // Draw a bright, visible circle for each dot
            this.trajectoryGraphics.fillStyle(0xffffff, 0.8);
            this.trajectoryGraphics.fillCircle(x, y, 4);

            // Move to next dot position
            x += velocity.x * 0.04;
            y += velocity.y * 0.04;

            // Stop if dot goes off screen
            if (x < 0 || x > this.cameras.main.width || y < 0 || y > this.cameras.main.height) {
                break;
            }
        }
    }

    launchBalls(dragVector) {
        this.isLaunching = true;

        const velocity = {
            x: dragVector.x * 3,
            y: dragVector.y * 3
        };

        // Clamp velocity
        const speed = Math.sqrt(velocity.x ** 2 + velocity.y ** 2);
        if (speed < GameConfig.ball.minSpeed) {
            velocity.x = (velocity.x / speed) * GameConfig.ball.minSpeed;
            velocity.y = (velocity.y / speed) * GameConfig.ball.minSpeed;
        } else if (speed > GameConfig.ball.maxSpeed) {
            velocity.x = (velocity.x / speed) * GameConfig.ball.maxSpeed;
            velocity.y = (velocity.y / speed) * GameConfig.ball.maxSpeed;
        }

        // Launch all balls with delay
        for (let i = 0; i < this.totalBalls; i++) {
            this.time.delayedCall(i * GameConfig.ball.launchDelay, () => {
                this.createBall(velocity);
                this.soundManager.playBallLaunch();
            });
        }
    }

    createBall(velocity) {
        const ball = this.add.circle(this.launchPoint.x, this.launchPoint.y, GameConfig.ball.radius, 0xffffff);

        this.physics.add.existing(ball);
        ball.body.setBounce(1, 1);
        ball.body.setCollideWorldBounds(true);
        ball.body.setVelocity(velocity.x, velocity.y);
        ball.body.setCircle(GameConfig.ball.radius);

        // Collisions
        this.physics.add.collider(ball, this.bricks, this.onBallHitBrick, null, this);
        this.physics.add.overlap(ball, this.ground, () => this.onBallHitGround(ball), null, this);

        this.activeBalls.push(ball);
    }

    onBallHitBrick(ball, brick) {
        // Reduce brick health
        let health = brick.getData('health');
        health--;
        brick.setData('health', health);

        // Update visuals
        const healthText = brick.getData('healthText');
        if (healthText) {
            healthText.setText(health.toString());
        }

        // Play sound
        if (health <= 0) {
            this.soundManager.playBrickDestroy();
            this.destroyBrick(brick);
        } else {
            this.soundManager.playBrickHit();
        }

        // Maintain ball speed
        const currentSpeed = Math.sqrt(ball.body.velocity.x ** 2 + ball.body.velocity.y ** 2);
        if (currentSpeed < GameConfig.ball.minSpeed) {
            const factor = GameConfig.ball.minSpeed / currentSpeed;
            ball.body.setVelocity(ball.body.velocity.x * factor, ball.body.velocity.y * factor);
        }
    }

    destroyBrick(brick) {
        // Add score
        const row = brick.getData('row');
        const points = (1 + row) * GameConfig.scoring.baseScore;
        this.score += points;
        this.scoreText.setText(`Score: ${this.score}`);

        // Remove brick
        const index = this.bricks.indexOf(brick);
        if (index > -1) {
            this.bricks.splice(index, 1);
        }

        brick.destroy();

        // Check level complete
        if (this.bricks.length === 0) {
            this.levelComplete();
        }
    }

    onBallHitGround(ball) {
        // Remove ball
        const index = this.activeBalls.indexOf(ball);
        if (index > -1) {
            this.activeBalls.splice(index, 1);
        }

        ball.destroy();

        // Check if all balls are gone
        if (this.activeBalls.length === 0) {
            this.isLaunching = false;
            this.onAllBallsLost();
        }
    }

    onAllBallsLost() {
        this.ballsRemaining--;
        this.ballsText.setText(`Balls: ${this.ballsRemaining}/${this.totalBalls}`);

        if (this.ballsRemaining <= 0) {
            // Check game over condition
            const lowestBrick = this.getLowestBrickY();
            if (lowestBrick && lowestBrick > this.launchPoint.y - 100) {
                this.triggerGameOver();
            } else {
                // Move bricks down and continue
                this.moveBricksDown();
                this.ballsRemaining = this.totalBalls;
                this.ballsText.setText(`Balls: ${this.ballsRemaining}/${this.totalBalls}`);
            }
        }

        this.saveProgress();
    }

    getLowestBrickY() {
        if (this.bricks.length === 0) return null;

        let lowest = 0;
        this.bricks.forEach(brick => {
            if (brick.y > lowest) {
                lowest = brick.y;
            }
        });

        return lowest;
    }

    moveBricksDown() {
        const moveDistance = GameConfig.brick.height + GameConfig.brick.spacing;

        this.bricks.forEach(brick => {
            brick.y += moveDistance;
        });

        // Check if any brick reached danger zone
        const lowestBrick = this.getLowestBrickY();
        if (lowestBrick && lowestBrick >= this.launchPoint.y - 50) {
            this.triggerGameOver();
        }
    }

    levelComplete() {
        this.soundManager.playLevelComplete();

        // Reward
        this.totalBalls++;
        this.currentLevel++;

        // Show level complete message
        const { width, height } = this.cameras.main;
        const message = this.add.text(width / 2, height / 2, `LEVEL ${this.currentLevel - 1} COMPLETE!\n+1 Ball`, {
            font: 'bold 48px Arial',
            fill: '#ffffff',
            align: 'center'
        });
        message.setOrigin(0.5);

        // Show interstitial ad every 3 levels
        if (this.currentLevel % GameConfig.ads.interstitialFrequency === 0) {
            this.time.delayedCall(2000, () => {
                this.adManager.showInterstitial(() => {
                    this.nextLevel();
                });
            });
        } else {
            this.time.delayedCall(2000, () => {
                this.nextLevel();
            });
        }
    }

    nextLevel() {
        this.ballsRemaining = this.totalBalls;
        this.saveProgress();
        this.scene.restart();
    }

    triggerGameOver() {
        if (this.gameOver) return;

        this.gameOver = true;
        this.soundManager.playGameOver();

        // Check if new high score
        const isNewHigh = this.storageManager.setHighScore(this.score);

        // Add to leaderboard
        this.storageManager.addToLeaderboard('Player', this.score, this.currentLevel);

        // Reset progress
        this.storageManager.resetProgress();

        // Go to game over scene
        this.scene.start('GameOverScene', {
            score: this.score,
            level: this.currentLevel,
            isNewHigh: isNewHigh
        });
    }

    saveProgress() {
        this.storageManager.setCurrentLevel(this.currentLevel);
        this.storageManager.setCurrentScore(this.score);
        this.storageManager.setTotalBalls(this.totalBalls);
    }

    showPauseMenu() {
        // Create pause overlay in new scene
        this.scene.launch('PauseScene', { gameScene: this });
    }

    update() {
        // Maintain ball speeds
        this.activeBalls.forEach(ball => {
            const speed = Math.sqrt(ball.body.velocity.x ** 2 + ball.body.velocity.y ** 2);

            if (speed < GameConfig.ball.minSpeed) {
                const factor = GameConfig.ball.minSpeed / speed;
                ball.body.setVelocity(ball.body.velocity.x * factor, ball.body.velocity.y * factor);
            } else if (speed > GameConfig.ball.maxSpeed) {
                const factor = GameConfig.ball.maxSpeed / speed;
                ball.body.setVelocity(ball.body.velocity.x * factor, ball.body.velocity.y * factor);
            }
        });
    }
}
