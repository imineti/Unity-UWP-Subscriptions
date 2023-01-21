# Unity-UWP-Subscriptions
Support for Unity UWP Subscriptions  
If you feel lead to buy me a beer or a pizza my paypal is kperil94@gmail.com and my cashapp is £STR4NGEWAYS
\
\
Instructions:  
\
Place the MicrosoftStoreSubscription.cs script in your project
\
\
In your code create a reference to a new instance of the MicrosoftStoreSubscription Class and create some strings for your UWP subscription product ID's, you also need some bools to check if subscribed  
(each product should have its own bool unless like me all subscriptions unlock the same features)  
```
MicrosoftStoreSubscription _mss = new MicrosoftStoreSubscription(); // Reference to a new instance of MicrosoftStoreSubscription class
string _mss_monthSub_ID = "KD840SJDI483"; // these are random letters and numbers swap these for your actual subscription product ID
string _mss_yearSub_ID = "LE492UDN49WI"; // these are random letters and numbersswap these for your actual subscription product ID

hasSubscription = false; // false until we check
```
\
\
In your code you need a method to check for subscriptions  
you also need a seperate method to check for UWPsubscriptions which uses async
```
public void CheckSubscriptions()
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    CheckSubscriptionsUWPAsync();
#else
    // This is where your standard Unity IAP subscription check goes (if you need), unity IAP subscriptions already work on other platforms just not UWP
#endif
}

public async void CheckSubscriptionsUWPAsync()
{
    hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_yearSub_ID);
    if (hasSubscription)
    {
        //----Has subscription, do stuff
        // You can get the DateTime of the expiry date using _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
        return; // I only use return because like I said both my subscriptions unlock the same features so there's no point checking the next one
    }

    hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_monthSub_ID);
    if (hasSubscription)
    {
        //----Has subscription, do stuff
        // You can get the DateTime of the expiry date using _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
    }
}
```
\
\
You should also have a method to prompt the purchase of subscriptions
```
public async void BuySubscription_OneMonth()
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    await _mss.PromptUserToPurchase(_mss_monthSub_ID);
    CheckSubscriptions();
#else
    // This is where your standard Unity IAP subscription purchase goes (if you use it)
    // Unity IAP subscriptions already work on other platforms
#endif
}

public async void BuySubscription_OneYear()
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    await _mss.PromptUserToPurchase(_mss_yearSub_ID);
    CheckSubscriptions();
#else
    // This is where your standard Unity IAP subscription purchase goes (if you use it)
    // Unity IAP subscriptions already work on other platforms
#endif
}
```
