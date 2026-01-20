// Sound Manager using Web Audio API and Vibration API
class SoundManager {
    constructor() {
        this.sounds = {};
        this.audioContext = null;
        this.enabled = true;
        this.musicVolume = 0.3;
        this.sfxVolume = 0.7;
        this.hapticsEnabled = true;

        // Initialize Audio Context (user interaction required)
        this.initAudioContext();
    }

    initAudioContext() {
        try {
            window.AudioContext = window.AudioContext || window.webkitAudioContext;
            this.audioContext = new AudioContext();
        } catch (e) {
            console.warn('Web Audio API not supported', e);
        }
    }

    // Generate simple tones (no external audio files needed)
    playTone(frequency, duration, volume = 0.5, type = 'sine') {
        if (!this.enabled || !this.audioContext) return;

        const oscillator = this.audioContext.createOscillator();
        const gainNode = this.audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(this.audioContext.destination);

        oscillator.frequency.value = frequency;
        oscillator.type = type;

        gainNode.gain.setValueAtTime(volume * this.sfxVolume, this.audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, this.audioContext.currentTime + duration);

        oscillator.start(this.audioContext.currentTime);
        oscillator.stop(this.audioContext.currentTime + duration);
    }

    // Sound effects
    playBallLaunch() {
        this.playTone(400, 0.1, 0.3, 'sine');
        this.vibrate(20);
    }

    playBallBounce() {
        this.playTone(300, 0.05, 0.2, 'square');
    }

    playBrickHit() {
        this.playTone(500, 0.08, 0.4, 'square');
        this.vibrate(10);
    }

    playBrickDestroy() {
        this.playTone(800, 0.15, 0.5, 'sine');
        this.vibrate(30);
    }

    playLevelComplete() {
        // Play ascending tones
        this.playTone(523, 0.15, 0.4, 'sine'); // C5
        setTimeout(() => this.playTone(659, 0.15, 0.4, 'sine'), 100); // E5
        setTimeout(() => this.playTone(784, 0.2, 0.5, 'sine'), 200);  // G5
        this.vibrate(50);
    }

    playGameOver() {
        // Play descending tones
        this.playTone(659, 0.2, 0.4, 'sine');
        setTimeout(() => this.playTone(523, 0.2, 0.4, 'sine'), 150);
        setTimeout(() => this.playTone(392, 0.3, 0.5, 'sine'), 300);
        this.vibrate(100);
    }

    playButtonClick() {
        this.playTone(600, 0.05, 0.3, 'square');
        this.vibrate(5);
    }

    // Haptic feedback
    vibrate(duration) {
        if (!this.hapticsEnabled) return;

        if (navigator.vibrate) {
            navigator.vibrate(duration);
        }
    }

    vibratePattern(pattern) {
        if (!this.hapticsEnabled) return;

        if (navigator.vibrate) {
            navigator.vibrate(pattern);
        }
    }

    // Settings
    setEnabled(enabled) {
        this.enabled = enabled;
    }

    setHapticsEnabled(enabled) {
        this.hapticsEnabled = enabled;
    }

    setSfxVolume(volume) {
        this.sfxVolume = Math.max(0, Math.min(1, volume));
    }

    setMusicVolume(volume) {
        this.musicVolume = Math.max(0, Math.min(1, volume));
    }

    isEnabled() {
        return this.enabled;
    }

    isHapticsEnabled() {
        return this.hapticsEnabled;
    }

    getSfxVolume() {
        return this.sfxVolume;
    }

    getMusicVolume() {
        return this.musicVolume;
    }
}
