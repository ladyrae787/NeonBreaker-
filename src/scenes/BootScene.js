// Boot Scene - Loads assets and initializes managers
class BootScene extends Phaser.Scene {
    constructor() {
        super({ key: 'BootScene' });
    }

    preload() {
        // Show loading progress
        const progressBar = this.add.graphics();
        const progressBox = this.add.graphics();
        progressBox.fillStyle(0x222222, 0.8);
        progressBox.fillRect(240, 620, 240, 30);

        const loadingText = this.make.text({
            x: 360,
            y: 600,
            text: 'Loading...',
            style: {
                font: '20px monospace',
                fill: '#ffffff'
            }
        });
        loadingText.setOrigin(0.5, 0.5);

        const percentText = this.make.text({
            x: 360,
            y: 635,
            text: '0%',
            style: {
                font: '18px monospace',
                fill: '#ffffff'
            }
        });
        percentText.setOrigin(0.5, 0.5);

        this.load.on('progress', (value) => {
            percentText.setText(parseInt(value * 100) + '%');
            progressBar.clear();
            progressBar.fillStyle(0x4DB8FF, 1);
            progressBar.fillRect(250, 625, 220 * value, 20);
        });

        this.load.on('complete', () => {
            progressBar.destroy();
            progressBox.destroy();
            loadingText.destroy();
            percentText.destroy();
        });

        // Load any assets here (currently using procedural graphics)
        // this.load.image('ball', 'assets/images/ball.png');
        // this.load.image('brick', 'assets/images/brick.png');
    }

    create() {
        // Initialize managers
        this.registry.set('storageManager', new StorageManager());
        this.registry.set('soundManager', new SoundManager());
        this.registry.set('adManager', new AdManager());

        // Hide loading indicator
        const loadingElement = document.getElementById('loading');
        if (loadingElement) {
            loadingElement.style.display = 'none';
        }

        // Resume audio context on first user interaction
        this.input.once('pointerdown', () => {
            const soundManager = this.registry.get('soundManager');
            if (soundManager && soundManager.audioContext) {
                soundManager.audioContext.resume();
            }
        });

        // Go to main menu
        this.scene.start('MainMenuScene');
    }
}
