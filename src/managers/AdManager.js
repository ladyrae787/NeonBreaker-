// Ad Manager for AdMob integration (via Capacitor)
class AdManager {
    constructor() {
        this.adsEnabled = GameConfig.ads.enabled;
        this.testMode = GameConfig.ads.testMode;
        this.interstitialLoaded = false;
        this.rewardedLoaded = false;
        this.adsRemoved = false;

        // Check if ads were purchased to be removed
        if (typeof(Storage) !== "undefined") {
            this.adsRemoved = localStorage.getItem('adsRemoved') === 'true';
        }

        this.initialize();
    }

    initialize() {
        // Check if running in Capacitor (mobile) or web
        if (window.Capacitor) {
            // Mobile: Use AdMob plugin
            this.initializeMobileAds();
        } else {
            // Web: Show placeholder ads
            console.log('Running in web mode - using placeholder ads');
        }
    }

    initializeMobileAds() {
        // This will work when the app is built with Capacitor
        // For now, we'll use placeholders

        console.log('AdMob initialized (test mode:', this.testMode + ')');

        // In a real Capacitor app, you would use:
        // import { AdMob } from '@capacitor-community/admob';
        // AdMob.initialize({ testingDevices: ['DEVICE_ID'] });
    }

    // Banner Ad
    showBanner() {
        if (!this.adsEnabled || this.adsRemoved) return;

        if (window.Capacitor && window.AdMob) {
            // Mobile AdMob banner
            window.AdMob.showBanner({
                adId: GameConfig.ads.bannerId,
                position: 'BOTTOM_CENTER'
            });
        } else {
            // Web placeholder
            console.log('Show banner ad');
        }
    }

    hideBanner() {
        if (window.Capacitor && window.AdMob) {
            window.AdMob.hideBanner();
        }
    }

    // Interstitial Ad
    loadInterstitial() {
        if (!this.adsEnabled || this.adsRemoved) return;

        if (window.Capacitor && window.AdMob) {
            window.AdMob.prepareInterstitial({
                adId: GameConfig.ads.interstitialId
            }).then(() => {
                this.interstitialLoaded = true;
            });
        } else {
            // Web: simulate loading
            this.interstitialLoaded = true;
        }
    }

    showInterstitial(onComplete) {
        if (!this.adsEnabled || this.adsRemoved) {
            if (onComplete) onComplete();
            return;
        }

        if (window.Capacitor && window.AdMob && this.interstitialLoaded) {
            window.AdMob.showInterstitial().then(() => {
                this.interstitialLoaded = false;
                this.loadInterstitial(); // Preload next
                if (onComplete) onComplete();
            });
        } else {
            // Web placeholder
            console.log('Show interstitial ad');
            setTimeout(() => {
                if (onComplete) onComplete();
            }, 1000);
        }
    }

    // Rewarded Video Ad
    loadRewarded() {
        if (!this.adsEnabled) return;

        if (window.Capacitor && window.AdMob) {
            window.AdMob.prepareRewardVideoAd({
                adId: GameConfig.ads.rewardedId
            }).then(() => {
                this.rewardedLoaded = true;
            });
        } else {
            // Web: simulate loading
            this.rewardedLoaded = true;
        }
    }

    showRewarded(onReward, onDismiss) {
        if (!this.adsEnabled) {
            if (onDismiss) onDismiss();
            return;
        }

        if (window.Capacitor && window.AdMob && this.rewardedLoaded) {
            window.AdMob.showRewardVideoAd().then((result) => {
                if (result.rewarded) {
                    if (onReward) onReward();
                } else {
                    if (onDismiss) onDismiss();
                }
                this.rewardedLoaded = false;
                this.loadRewarded(); // Preload next
            });
        } else {
            // Web placeholder - simulate watching ad
            console.log('Show rewarded video ad');

            // Simulate 3-second ad
            const adOverlay = document.createElement('div');
            adOverlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.9);
                color: white;
                display: flex;
                flex-direction: column;
                justify-content: center;
                align-items: center;
                z-index: 10000;
                font-family: Arial, sans-serif;
            `;

            const adText = document.createElement('div');
            adText.textContent = 'Rewarded Ad (Demo)';
            adText.style.fontSize = '24px';
            adText.style.marginBottom = '20px';

            const countdown = document.createElement('div');
            countdown.style.fontSize = '48px';
            countdown.style.marginBottom = '20px';

            const skipBtn = document.createElement('button');
            skipBtn.textContent = 'Skip (5s)';
            skipBtn.disabled = true;
            skipBtn.style.cssText = `
                padding: 15px 30px;
                font-size: 18px;
                background: #666;
                color: white;
                border: none;
                border-radius: 5px;
                cursor: not-allowed;
            `;

            adOverlay.appendChild(adText);
            adOverlay.appendChild(countdown);
            adOverlay.appendChild(skipBtn);
            document.body.appendChild(adOverlay);

            let timeLeft = 5;
            countdown.textContent = timeLeft;

            const timer = setInterval(() => {
                timeLeft--;
                countdown.textContent = timeLeft;

                if (timeLeft <= 0) {
                    clearInterval(timer);
                    skipBtn.disabled = false;
                    skipBtn.style.background = '#4DB8FF';
                    skipBtn.style.cursor = 'pointer';
                    skipBtn.textContent = 'Claim Reward';
                }
            }, 1000);

            skipBtn.onclick = () => {
                document.body.removeChild(adOverlay);
                if (timeLeft <= 0) {
                    if (onReward) onReward();
                } else {
                    if (onDismiss) onDismiss();
                }
            };
        }
    }

    isRewardedAvailable() {
        return this.rewardedLoaded;
    }

    // Remove ads (after IAP)
    removeAds() {
        this.adsRemoved = true;
        if (typeof(Storage) !== "undefined") {
            localStorage.setItem('adsRemoved', 'true');
        }
        this.hideBanner();
    }

    areAdsRemoved() {
        return this.adsRemoved;
    }
}
