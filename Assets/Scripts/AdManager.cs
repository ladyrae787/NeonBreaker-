using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Ad Settings")]
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] private string iosGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] private bool testMode = true;

    [Header("Ad Unit IDs")]
    [SerializeField] private string bannerAdUnitId = "Banner_Android";
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android";
    [SerializeField] private string rewardedAdUnitId = "Rewarded_Android";

    [Header("UI References")]
    [SerializeField] private Button rewardedAdButton;
    [SerializeField] private GameObject adLoadingPanel;

    private string gameId;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeAds();
    }

    private void InitializeAds()
    {
        #if UNITY_ANDROID
            gameId = androidGameId;
        #elif UNITY_IOS
            gameId = iosGameId;
        #else
            gameId = androidGameId;
        #endif

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        isInitialized = true;

        // Load ads
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }

    // ===== BANNER ADS =====
    private void LoadBannerAd()
    {
        if (!isInitialized) return;

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(bannerAdUnitId);
    }

    public void ShowBannerAd()
    {
        if (!isInitialized) return;

        Advertisement.Banner.Show(bannerAdUnitId);
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    // ===== INTERSTITIAL ADS =====
    private void LoadInterstitialAd()
    {
        if (!isInitialized) return;

        Advertisement.Load(interstitialAdUnitId, this);
    }

    public void ShowInterstitialAd()
    {
        if (!isInitialized) return;

        Advertisement.Show(interstitialAdUnitId, this);
    }

    // ===== REWARDED ADS =====
    private void LoadRewardedAd()
    {
        if (!isInitialized) return;

        Advertisement.Load(rewardedAdUnitId, this);
    }

    public void ShowRewardedAd()
    {
        if (!isInitialized)
        {
            Debug.Log("Ads not initialized yet.");
            return;
        }

        if (adLoadingPanel != null)
            adLoadingPanel.SetActive(true);

        Advertisement.Show(rewardedAdUnitId, this);
    }

    public void ShowRewardedAdButton()
    {
        if (rewardedAdButton != null)
        {
            rewardedAdButton.gameObject.SetActive(true);
            rewardedAdButton.onClick.RemoveAllListeners();
            rewardedAdButton.onClick.AddListener(ShowRewardedAd);
        }
    }

    public void HideRewardedAdButton()
    {
        if (rewardedAdButton != null)
        {
            rewardedAdButton.gameObject.SetActive(false);
        }
    }

    // ===== AD CALLBACKS =====
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad Loaded: {placementId}");

        if (placementId == rewardedAdUnitId && rewardedAdButton != null)
        {
            rewardedAdButton.interactable = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {placementId}: {error} - {message}");

        // Retry loading
        if (placementId == interstitialAdUnitId)
        {
            Invoke(nameof(LoadInterstitialAd), 5f);
        }
        else if (placementId == rewardedAdUnitId)
        {
            Invoke(nameof(LoadRewardedAd), 5f);
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {placementId}: {error} - {message}");

        if (adLoadingPanel != null)
            adLoadingPanel.SetActive(false);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad Started: {placementId}");

        if (adLoadingPanel != null)
            adLoadingPanel.SetActive(false);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad Clicked: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == rewardedAdUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded ad completed. Granting reward.");
            GrantReward();
        }
        else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
        {
            Debug.Log("Rewarded ad was skipped.");
        }

        // Reload ads
        if (placementId == interstitialAdUnitId)
        {
            LoadInterstitialAd();
        }
        else if (placementId == rewardedAdUnitId)
        {
            LoadRewardedAd();
            HideRewardedAdButton();
        }
    }

    private void GrantReward()
    {
        // Grant reward - continue game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinueWithRewardedAd();
        }
    }

    private void OnDestroy()
    {
        if (rewardedAdButton != null)
        {
            rewardedAdButton.onClick.RemoveAllListeners();
        }
    }
}
