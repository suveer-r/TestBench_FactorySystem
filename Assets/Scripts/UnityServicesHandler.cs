using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ProductIds
{
    public const string gems20 = "g20";
    public const string gems50 = "g50";
    public const string gems100 = "g100";
    public const string no_ads = "no_ads";
    public const string g5_ad = "g5_ad";
    public const string no_ads_for_week = "no_ads_for_week";
}

[DefaultExecutionOrder(-1)]
public class UnityServicesHandler : MonoBehaviour, IDetailedStoreListener, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_EDITOR
    [Header("IAP")]
    [SerializeField] private GameObject iapPage;
    [SerializeField] private Button toggleIapPage;

    [Header("Gaming Services")]
    [SerializeField] private string environment = "staging";

    [Header("Unity Ads")]
    [SerializeField] private string gameId = "3003911";

    IStoreController storeController;
    private void Awake()
    {
        iapPage.SetActive(false);
        toggleIapPage.onClick.AddListener(ToggleIapPage);

        InitializeGamingServices(OnSuccess, OnError);
    }

    private void Start()
    {
        InitializePurchasing();

        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode: true, this); // Set testMode to false for production
        }
    }

    private bool IsSubscribedTo(Product subscription)
    {
        if (subscription.receipt == null)
        {
            Debug.Log("No Receipt");
            return false;
        }

        var subscriptionManager = new SubscriptionManager(subscription, null);

        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }

    #region Unity Gaming Services
    private void InitializeGamingServices(Action onSuccess, Action<string> onError)
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);

            UnityServices.InitializeAsync(options).ContinueWith(task => onSuccess());
        }
        catch (Exception exception)
        {
            onError(exception.Message);
        }
    }
    private void OnSuccess()
    {
        Debug.Log("Gaming Service Initialized");
    }

    void OnError(string message)
    {
        Debug.LogError("Gaming Service FAILED TO Initialized");
    }
    #endregion

    #region IDetailedStoreListener interface
    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(ProductIds.gems20, ProductType.Consumable);
        builder.AddProduct(ProductIds.gems50, ProductType.Consumable);
        builder.AddProduct(ProductIds.gems100, ProductType.Consumable);
        builder.AddProduct(ProductIds.no_ads, ProductType.NonConsumable);
        builder.AddProduct(ProductIds.g5_ad, ProductType.Consumable);
        builder.AddProduct(ProductIds.no_ads_for_week, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;

        Debug.Log("INIT");

        Product adFreeSubs = storeController.products.WithID(ProductIds.no_ads_for_week);

        try
        {
            var isSubscribed = IsSubscribedTo(adFreeSubs);
        }
        catch (StoreSubscriptionInfoNotSupportedException)
        {
            return;
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Marked Obsolete
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // Marked Obsolete
        OnPurchaseFailed(product, null);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        OnPurchaseSuccess(product);

        return PurchaseProcessingResult.Complete;
    }
    #endregion


    #region UNITY_ADS
    public void OnInitializationComplete()
    {
        Debug.Log("Init Success");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == ProductIds.g5_ad)
        {
            Advertisement.Show(ProductIds.g5_ad, this);
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == ProductIds.g5_ad)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Rewarded ad completed, rewarding player.");
                RewardGems(5);
            }
            else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
            {
                Debug.Log("Rewarded ad skipped.");
            }
            else if (showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
            {
                Debug.Log("Rewarded ad failed to show.");
            }
        }
    }
    #endregion

    private void RewardGems(int gems)
    {
        DataHandler.GemsCollected += gems;
        ToggleIapPage();
    }
    public void OnPurchaseSuccess(Product product)
    {
        Debug.Log("Product Purchased: " + product.definition.id.ToString());

        switch (product.definition.id)
        {
            case ProductIds.gems20:
                RewardGems(20);
                break;

            case ProductIds.gems50:
                RewardGems(50);
                break;

            case ProductIds.gems100:
                RewardGems(100);
                break;

            case ProductIds.no_ads:
                DateTimeHandler.GetCurrentDateTIme((TimeResponse d) =>
                {
                    DateTime timeNow = DateTime.Parse(d.datetime);
                    if (string.IsNullOrEmpty(DataHandler.AdFreeDayStartTime) || timeNow > DateTime.Parse(DataHandler.AdFreeDayStartTime).AddHours(24))
                    {
                        DataHandler.AdFreeDayStartTime = d.datetime;
                    }
                }, null);
                break;

            case ProductIds.g5_ad:
                if (Advertisement.isInitialized)
                {
                    Advertisement.Load(ProductIds.g5_ad, this);
                }
                else
                {
                    Debug.Log("Rewarded video ad is not ready yet.");
                }
                break;

            case ProductIds.no_ads_for_week:
                DateTimeHandler.GetCurrentDateTIme((TimeResponse d) =>
                {
                    DateTime timeNow = DateTime.Parse(d.datetime);
                    if (string.IsNullOrEmpty(DataHandler.AdFreeWeekStartTime) || timeNow > DateTime.Parse(DataHandler.AdFreeWeekStartTime).AddDays(7))
                    {
                        DataHandler.AdFreeWeekStartTime = d.datetime;
                    }
                }, null);
                break;
        }
    }

    public void PurchaseProduct(string id)
    {
        /// FIXME: VALIDATE PURCHASE HERE. OPEN A POP UP OR VALIDATE PAYMENTS, ETC.
        storeController.InitiatePurchase(id);
    }

    private void ToggleIapPage()
    {
        iapPage.SetActive(!iapPage.activeSelf);
    }

#endif
}
