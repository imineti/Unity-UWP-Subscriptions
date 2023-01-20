# Unity-UWP-Subscriptions
Support for Unity UWP Subscriptions  
\
If you feel lead to buy me a beer or a pizza my paypal is kperil94@gmail.com and my cashapp is £STR4NGEWAYS
\
\
\
\
Make sure to place the MicrosoftStoreSubscription.cs script somewhere in your project where it can be referenced by the assembly
\
\
\
\
In your code create a reference to a new instance of the MicrosoftStoreSubscription Class and create some strings for your UWP subscription product ID's  
you also need some bools to check if subscribed (each product should have its own bool unless like me all subscriptions unlock the same features)  
```
MicrosoftStoreSubscription _mss = new MicrosoftStoreSubscription(); // Reference to a new instance of MicrosoftStoreSubscription class
string _mss_monthSub_ID = "KD840SJDI483"; // these are random letters and numbers swap these for your actual subscription product ID
string _mss_yearSub_ID = "LE492UDN49WI"; // these are random letters and numbersswap these for your actual subscription product ID

hasSubscription = false; // false until we check
```
\
\
In your code you should have a method to check subscriptions
```
public async void CheckSubscriptions()
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_yearSub_ID);
    
    if (hasSubscription)
    {
        //----if has this subscription do stuff
        // you can also get the DateTime of the expiration date with _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
        return; // remove this if you still need to check the other subscriptions (I don't because all my subscriptions unlock the same features)
    }
    
    hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_monthSub_ID);

    if (hasSubscription)
    {
        //----if has this subscription do stuff
        // you can also get the DateTime of the expiration date with _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
        return; // remove this if you still need to check the other subscriptions (I don't because all my subscriptions unlock the same features)
    }
#else
    // This is where your standard Unity IAP subscription check goes (if you need), unity IAP subscriptions already work on other platforms just not UWP
#endif
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
    await CheckSubscriptions();
#else
    // This is where your standard Unity IAP subscription purchase goes (if you use it)
    // Unity IAP subscriptions already work on other platforms
#endif
}

public async void BuySubscription_OneYear()
{
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
    await _mss.PromptUserToPurchase(_mss_yearSub_ID);
    await CheckSubscriptions();
#else
    // This is where your standard Unity IAP subscription purchase goes (if you use it)
    // Unity IAP subscriptions already work on other platforms
#endif
}
```
