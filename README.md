# Unity-UWP-Subscriptions
Support for Unity UWP Subscriptions


Make sure to place the MicrosoftStoreSubscription.cs script somewhere in your project where it can be referenced by the assembly

In your code create a reference to a new instance of the MicrosoftStoreSubscription class and create some strings for your UWP subscription product ID's
```
MicrosoftStoreSubscription _mss = new MicrosoftStoreSubscription(); // Reference to a new instance of MicrosoftStoreSubscription class
string _mss_monthSub_ID = "KD840SJDI483"; // these are random letters and numbers swap these for your actual subscription product ID
string _mss_yearSub_ID = "LE492UDN49WI"; // these are random letters and numbersswap these for your actual subscription product ID
```





    //----Check subscriptions to see if hasSubscription should be true
    // In my case you can have a yearly OR monthly subscription and since a yearly subscription lasts longer I will return if they have it because there would be
    // no point checking for a monthly subscription when I already have the information I need
    // BUT if I had multiple subscriptions for different things within the app I would check them all and set unique bool variables for them all
    public async void CheckSubscriptions()
    {
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
        hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_yearSub_ID); // checks if subscribed to this subscription using the product ID
                    // hasSubscription is a bool variable I use to see if a user is subscribed you can replace this with your own bool variable if you have one
                    // I check if hasSubscription is true when a user tries to use a pro feature
                    // CheckIfUserHasSubscription() returns true or false and is an async Task<bool> method
  
        if (hasSubscription)
        {
            //----if has this subscription do stuff
            // you can also get the DateTime of the expiration date with _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
            return; // remove this if you still need to check the other subscriptions
        }
  
        hasSubscription = await _mss.CheckIfUserHasSubscription(_mss_monthSub_ID); // checks if subscribed to this subscription using the product ID
                    // hasSubscription is a bool variable I use to see if a user is subscribed you can replace this with your own bool variable if you have one
                    // I check if hasSubscription is true when a user tries to use a pro feature
                    // CheckIfUserHasSubscription() returns true or false and is an async Task<bool> method
  
        if (hasSubscription)
        {
            //----if has this subscription do stuff
            // you can also get the DateTime of the expiration date with _mss.DateTimeOffsetToDateTime(_mss.expirationDateOfLastCheckedSubscription);
        }
#else
        // This is where your standard Unity IAP subscription check goes (if you need), unity IAP subscriptions already work on other platforms just not UWP
#endif
    }
  
    public async void BuySubscription_OneMonth()
    {
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
        await _mss.PromptUserToPurchase(_mss_monthSub_ID);
#else
        // This is where your standard Unity IAP subscription purchase goes (if you need)
        // Unity IAP subscriptions already work on other platforms just not UWP
#endif
    }

    public async void BuySubscription_OneYear()
    {
#if ((UNITY_WSA && !UNITY_EDITOR) && ENABLE_WINMD_SUPPORT)
        await _mss.PromptUserToPurchase(_mss_yearSub_ID);
#else
        // This is where your standard Unity IAP subscription purchase goes (if you need)
        // Unity IAP subscriptions already work on other platforms just not UWP
#endif
    }
