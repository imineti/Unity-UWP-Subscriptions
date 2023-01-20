using UnityEngine;
using System;
using System.Runtime;
using System.Threading.Tasks;
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
using Windows.Services.Store;
#endif

public class MicrosoftStoreSubscription
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    private StoreContext context = StoreContext.GetDefault();
#endif
    public DateTimeOffset expirationDateOfLastCheckedSubscription = DateTimeOffset.Now;

    public async Task<bool> CheckIfUserHasSubscription(string _subscriptionStoreId)
    {
        bool _hasSubscription = false;
        await Task.Delay(1);

#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
        _hasSubscription = await CheckIfUserHasSubscriptionAsync(_subscriptionStoreId);
#endif
        return _hasSubscription;
    }

    public async Task PromptUserToPurchase(string _subscriptionStoreId)
    {
        await Task.Delay(1);

#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
        await PromptUserToPurchaseAsync(_subscriptionStoreId);
#endif
    }

#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    private async Task<bool> CheckIfUserHasSubscriptionAsync(string _subscriptionStoreId)
    {
        StoreAppLicense appLicense = await context.GetAppLicenseAsync();

        // Check if the customer has the rights to the subscription.
        foreach (var addOnLicense in appLicense.AddOnLicenses)
        {
            StoreLicense license = addOnLicense.Value;
            if (license.SkuStoreId.StartsWith(_subscriptionStoreId))
            {
                if (license.IsActive)
                {
                    // The expiration date is available in the license.ExpirationDate property.
                    expirationDateOfLastCheckedSubscription = license.ExpirationDate;
                    return true;
                }
            }
        }

        // The customer does not have a license to the subscription.
        return false;
    }

    private async Task<StoreProduct> GetSubscriptionProductAsync(string _subscriptionStoreId)
    {
        // Load the sellable add-ons for this app and check if the trial is still 
        // available for this customer. If they previously acquired a trial they won't 
        // be able to get a trial again, and the StoreProduct.Skus property will 
        // only contain one SKU.
        StoreProductQueryResult result =
            await context.GetAssociatedStoreProductsAsync(new string[] { "Durable" });

        if (result.ExtendedError != null)
        {
            Debug.LogError("GetSubscriptionProductAsync: Something went wrong while getting the add-ons. " +
                "ExtendedError: " + result.ExtendedError);
            return null;
        }

        // Look for the product that represents the subscription.
        foreach (var item in result.Products)
        {
            StoreProduct product = item.Value;
            if (product.StoreId == _subscriptionStoreId)
            {
                return product;
            }
        }

        Debug.LogError("GetSubscriptionProductAsync: The subscription was not found.");
        return null;
    }

    private async Task PromptUserToPurchaseAsync(string _subscriptionStoreId)
    {
        // Look for the product that represents the subscription.
        StoreProduct _subscriptionStoreProduct = await GetSubscriptionProductAsync(_subscriptionStoreId);

        if (_subscriptionStoreProduct == null)
        {
            Debug.LogError("PromptUserToPurchaseAsync: _subscriptionStoreProduct IS NULL");
            return;
        }

        if (_subscriptionStoreProduct != null)
        {
            // Request a purchase of the subscription product. If a trial is available it will be offered 
            // to the customer. Otherwise, the non-trial SKU will be offered.
            StorePurchaseResult result = await _subscriptionStoreProduct.RequestPurchaseAsync();

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null)
            {
                extendedError = result.ExtendedError.Message;
                Debug.LogError("PromptUserToPurchaseAsync: ExtendedError: - " + extendedError);
            }

            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    // Show a UI to acknowledge that the customer has purchased your subscription 
                    // and unlock the features of the subscription.
                    Debug.LogError("PromptUserToPurchaseAsync: Purchase successful");
                    break;

                case StorePurchaseStatus.NotPurchased:
                    Debug.LogError("PromptUserToPurchaseAsync: The purchase did not complete. " +
                        "The customer may have cancelled the purchase. ExtendedError: " + extendedError);
                    break;

                case StorePurchaseStatus.ServerError:
                case StorePurchaseStatus.NetworkError:
                    Debug.LogError("The purchase was unsuccessful due to a server or network error. " +
                        "ExtendedError: " + extendedError);
                    break;

                case StorePurchaseStatus.AlreadyPurchased:
                    Debug.LogError("The customer already owns this subscription." +
                            "ExtendedError: " + extendedError);
                    break;
            }
        }
    }
#endif

    public DateTime DateTimeOffsetToDateTime(DateTimeOffset _sourceTime)
    {
        DateTime _baseTime = DateTime.Now;
        DateTime _targetTime;

        // Convert UTC to DateTime value
        _sourceTime = new DateTimeOffset(_baseTime, TimeSpan.Zero);
        _targetTime = _sourceTime.DateTime;

        // Convert local time to DateTime value
        _sourceTime = new DateTimeOffset(_baseTime,
                                        TimeZoneInfo.Local.GetUtcOffset(_baseTime));
        _targetTime = _sourceTime.DateTime;

        // Convert Central Standard Time to a DateTime value
        try
        {
            TimeSpan offset = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time").GetUtcOffset(_baseTime);
            _sourceTime = new DateTimeOffset(_baseTime, offset);
            _targetTime = _sourceTime.DateTime;
            return _targetTime;
        }
        catch (TimeZoneNotFoundException)
        {
            Debug.Log("Unable to create DateTimeOffset based on U.S. Central Standard Time.");
            return DateTime.MinValue;
        }
    }
}
