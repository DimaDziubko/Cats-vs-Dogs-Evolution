using _Game.Core._GameMode;
using AppsFlyerConnector;
using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class CustomAppsFlyerObject : MonoBehaviour
    {

        [SerializeField] private bool _isDebug;
        [SerializeField] private string _devKeyAndroid;
        [SerializeField] private string _devKeyIOS;
        [SerializeField] private bool _isGetConversionData;
        [SerializeField] private bool _isSandbox;
        /// <summary>
        /// For IOS
        /// </summary>
        [SerializeField] private string _appID;


#if UNITY_IOS
        private string _devKey => _devKeyIOS;
#else // UNITY_ANDROID
        private string _devKey => _devKeyAndroid;

#endif

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // These fields are set from the editor so do not modify!
            //******************************//
            AppsFlyer.setIsDebug(_isDebug);

            AppsFlyer.initSDK(_devKey, _appID, _isGetConversionData ? this : null);
            //******************************/

            // Purchase connector implementation 
            AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
            AppsFlyerPurchaseConnector.setIsSandbox(_isSandbox);

            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            //Auto Send AF_Purchase
            //AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions, AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            AppsFlyerPurchaseConnector.build();

            AppsFlyerPurchaseConnector.startObservingTransactions();


            AppsFlyer.startSDK();

        }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
            // deserialize the string as a dictionnary, easy to manipulate
            Dictionary<string, object> dictionary = AFMiniJSON.Json.Deserialize(validationInfo) as Dictionary<string, object>;

            // if the platform is Android, you can create an object from the dictionnary 
#if UNITY_ANDROID
            if (dictionary.ContainsKey("productPurchase") && dictionary["productPurchase"] != null)
            {
                // Create an object from the JSON string.
                InAppPurchaseValidationResult iapObject = JsonUtility.FromJson<InAppPurchaseValidationResult>(validationInfo);
            }
            else if (dictionary.ContainsKey("subscriptionPurchase") && dictionary["subscriptionPurchase"] != null)
            {
                SubscriptionValidationResult iapObject = JsonUtility.FromJson<SubscriptionValidationResult>(validationInfo);
            }
#endif
        }

        public void SetCustomerID()
        {
            AppsFlyer.setCustomerUserId(GameMode.GetUniqUserID());
        }

        public void StartTrackAdRevenue()
        {
            AppsFlyerAdRevenue.start();


            Debug.Log($"AppsFlyerAnalyticServicePlatform:: AdRevenueConnector: AppsFlyerAdRevenue.start()");
#if DEVELOPMENT_BUILD
            AppsFlyerAdRevenue.setIsDebug(true);
#else
            AppsFlyerAdRevenue.setIsDebug(false);
#endif
        }

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
    }
}