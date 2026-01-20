using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    [Header("Product IDs")]
    public const string PRODUCT_REMOVE_ADS = "com.yourgame.removeads";
    public const string PRODUCT_EXTRA_BALLS_SMALL = "com.yourgame.balls_small";
    public const string PRODUCT_EXTRA_BALLS_MEDIUM = "com.yourgame.balls_medium";
    public const string PRODUCT_EXTRA_BALLS_LARGE = "com.yourgame.balls_large";

    [Header("UI References")]
    [SerializeField] private Button removeAdsButton;
    [SerializeField] private Button buyBallsSmallButton;
    [SerializeField] private Button buyBallsMediumButton;
    [SerializeField] private Button buyBallsLargeButton;
    [SerializeField] private Text statusText;

    private bool isInitialized = false;
    private bool adsRemoved = false;

    private void Start()
    {
        InitializePurchasing();
        LoadPurchaseStatus();

        // Setup button listeners
        if (removeAdsButton != null)
            removeAdsButton.onClick.AddListener(() => BuyProductID(PRODUCT_REMOVE_ADS));

        if (buyBallsSmallButton != null)
            buyBallsSmallButton.onClick.AddListener(() => BuyProductID(PRODUCT_EXTRA_BALLS_SMALL));

        if (buyBallsMediumButton != null)
            buyBallsMediumButton.onClick.AddListener(() => BuyProductID(PRODUCT_EXTRA_BALLS_MEDIUM));

        if (buyBallsLargeButton != null)
            buyBallsLargeButton.onClick.AddListener(() => BuyProductID(PRODUCT_EXTRA_BALLS_LARGE));
    }

    public void InitializePurchasing()
    {
        if (isInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add products
        builder.AddProduct(PRODUCT_REMOVE_ADS, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_EXTRA_BALLS_SMALL, ProductType.Consumable);
        builder.AddProduct(PRODUCT_EXTRA_BALLS_MEDIUM, ProductType.Consumable);
        builder.AddProduct(PRODUCT_EXTRA_BALLS_LARGE, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP: Initialized successfully");
        storeController = controller;
        storeExtensionProvider = extensions;
        isInitialized = true;

        UpdateStatus("Store ready!");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"IAP: Initialization failed - {error}");
        UpdateStatus("Store unavailable");
    }

    public void BuyProductID(string productId)
    {
        if (!isInitialized)
        {
            UpdateStatus("Store not ready");
            return;
        }

        Product product = storeController.products.WithID(productId);

        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"Purchasing product: {product.definition.id}");
            UpdateStatus("Processing purchase...");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"Product {productId} not available for purchase");
            UpdateStatus("Product not available");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;

        Debug.Log($"Purchase successful: {productId}");

        // Handle different products
        if (productId == PRODUCT_REMOVE_ADS)
        {
            RemoveAds();
            UpdateStatus("Ads removed!");
        }
        else if (productId == PRODUCT_EXTRA_BALLS_SMALL)
        {
            GrantExtraBalls(5);
            UpdateStatus("5 extra balls added!");
        }
        else if (productId == PRODUCT_EXTRA_BALLS_MEDIUM)
        {
            GrantExtraBalls(15);
            UpdateStatus("15 extra balls added!");
        }
        else if (productId == PRODUCT_EXTRA_BALLS_LARGE)
        {
            GrantExtraBalls(50);
            UpdateStatus("50 extra balls added!");
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: {product.definition.storeSpecificId}, Reason: {failureReason}");
        UpdateStatus($"Purchase failed: {failureReason}");
    }

    private void RemoveAds()
    {
        adsRemoved = true;
        PlayerPrefs.SetInt("AdsRemoved", 1);
        PlayerPrefs.Save();

        // Disable ads
        AdManager adManager = FindObjectOfType<AdManager>();
        if (adManager != null)
        {
            adManager.HideBannerAd();
            adManager.HideRewardedAdButton();
        }

        // Hide remove ads button
        if (removeAdsButton != null)
        {
            removeAdsButton.gameObject.SetActive(false);
        }
    }

    private void GrantExtraBalls(int amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ballsCollected += amount;
        }
    }

    private void LoadPurchaseStatus()
    {
        adsRemoved = PlayerPrefs.GetInt("AdsRemoved", 0) == 1;

        if (adsRemoved)
        {
            RemoveAds();
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            Invoke(nameof(ClearStatus), 3f);
        }

        Debug.Log($"IAP Status: {message}");
    }

    private void ClearStatus()
    {
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    public bool AreAdsRemoved()
    {
        return adsRemoved;
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        if (removeAdsButton != null)
            removeAdsButton.onClick.RemoveAllListeners();

        if (buyBallsSmallButton != null)
            buyBallsSmallButton.onClick.RemoveAllListeners();

        if (buyBallsMediumButton != null)
            buyBallsMediumButton.onClick.RemoveAllListeners();

        if (buyBallsLargeButton != null)
            buyBallsLargeButton.onClick.RemoveAllListeners();
    }
}
