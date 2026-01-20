// Local Storage Manager for High Scores and Settings
class StorageManager {
    constructor() {
        this.storageAvailable = this.checkStorageAvailability();
    }

    checkStorageAvailability() {
        try {
            const test = '__storage_test__';
            localStorage.setItem(test, test);
            localStorage.removeItem(test);
            return true;
        } catch (e) {
            return false;
        }
    }

    // High Score Management
    getHighScore() {
        if (!this.storageAvailable) return 0;
        return parseInt(localStorage.getItem('highScore') || '0', 10);
    }

    setHighScore(score) {
        if (!this.storageAvailable) return;
        const currentHigh = this.getHighScore();
        if (score > currentHigh) {
            localStorage.setItem('highScore', score.toString());
            return true;
        }
        return false;
    }

    // Leaderboard Management
    getLeaderboard() {
        if (!this.storageAvailable) return [];
        const data = localStorage.getItem('leaderboard');
        return data ? JSON.parse(data) : [];
    }

    addToLeaderboard(name, score, level) {
        if (!this.storageAvailable) return;

        let leaderboard = this.getLeaderboard();

        leaderboard.push({
            name: name || 'Player',
            score: score,
            level: level,
            date: new Date().toISOString()
        });

        // Sort by score descending
        leaderboard.sort((a, b) => b.score - a.score);

        // Keep top 10
        leaderboard = leaderboard.slice(0, 10);

        localStorage.setItem('leaderboard', JSON.stringify(leaderboard));
    }

    // Settings Management
    getSetting(key, defaultValue) {
        if (!this.storageAvailable) return defaultValue;
        const value = localStorage.getItem(`setting_${key}`);
        return value !== null ? JSON.parse(value) : defaultValue;
    }

    setSetting(key, value) {
        if (!this.storageAvailable) return;
        localStorage.setItem(`setting_${key}`, JSON.stringify(value));
    }

    // Game Progress
    getTotalBalls() {
        return this.getSetting('totalBalls', 1);
    }

    setTotalBalls(balls) {
        this.setSetting('totalBalls', balls);
    }

    getCurrentLevel() {
        return this.getSetting('currentLevel', 1);
    }

    setCurrentLevel(level) {
        this.setSetting('currentLevel', level);
    }

    getCurrentScore() {
        return this.getSetting('currentScore', 0);
    }

    setCurrentScore(score) {
        this.setSetting('currentScore', score);
    }

    // Reset progress
    resetProgress() {
        this.setTotalBalls(1);
        this.setCurrentLevel(1);
        this.setCurrentScore(0);
    }

    // Clear all data
    clearAll() {
        if (!this.storageAvailable) return;
        localStorage.clear();
    }
}
