using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class AppsFlyerSettings : MonoBehaviour
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
        public string DevKey => _devKeyIOS;
#else // UNITY_ANDROID
        public string DevKey => _devKeyAndroid;
#endif
        public bool IsDebug => _isDebug;
        public bool IsGetConversionData => _isGetConversionData;
        public bool IsSandbox => _isSandbox;
        public string AppID => _appID;

        
//         private void Start()
//         {
//             InitAppsFlyer();
//         }
//         private void InitAppsFlyer()
//         {
//             AppsFlyer.setIsDebug(_isDebug);
//
//             SetCustomerID();
//
//             AppsFlyer.initSDK(DevKey, _appID, this);
//             AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
//             //#if DEVELOPMENT_BUILD
//             AppsFlyerPurchaseConnector.setIsSandbox(true);
//             //#else
//             //            AppsFlyerPurchaseConnector.setIsSandbox(false);
//             //#endif
//             AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
//             AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions,
//                 AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
//             AppsFlyerPurchaseConnector.build();
//             AppsFlyerPurchaseConnector.startObservingTransactions();
//
//
//             AppsFlyer.startSDK();
//
//             Invoke(nameof(StartAdRevenueConnector), 1);
//         }
//         private void StartAdRevenueConnector()
//         {
//             AppsFlyerAdRevenue.start();
// #if DEVELOPMENT_BUILD
//             AppsFlyerAdRevenue.setIsDebug(true);
// #else
//             AppsFlyerAdRevenue.setIsDebug(false);
// #endif
//             MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
//             MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
//             MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
//         }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
            // deserialize the string as a dictionnary, easy to manipulate
            Dictionary<string, object> dictionary = AFMiniJSON.Json.Deserialize(validationInfo) as Dictionary<string, object>;

            // if the platform is Android, you can create an object from the dictionnary 
            //#if UNITY_ANDROID
            //            if (dictionary.ContainsKey("productPurchase") && dictionary["productPurchase"] != null)
            //            {
            //                // Create an object from the JSON string.
            //                InAppPurchaseValidationResult iapObject = JsonUtility.FromJson<InAppPurchaseValidationResult>(validationInfo);
            //            }
            //            else if (dictionary.ContainsKey("subscriptionPurchase") && dictionary["subscriptionPurchase"] != null)
            //            {
            //                SubscriptionValidationResult iapObject = JsonUtility.FromJson<SubscriptionValidationResult>(validationInfo);
            //            }
            //#endif
        }
        // private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        // {
        //     Dictionary<string, string> additionalParams = new Dictionary<string, string>();
        //     additionalParams.Add(AFAdRevenueEvent.AD_UNIT, adInfo.AdUnitIdentifier);
        //     additionalParams.Add(AFAdRevenueEvent.AD_TYPE, adInfo.AdFormat);
        //     AppsFlyerAdRevenue.logAdRevenue(adInfo.NetworkName,
        //         AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
        //         adInfo.Revenue, "USD", additionalParams);
        // }
        // public void SetCustomerID()
        // {
        //     AppsFlyer.setCustomerUserId(GameMode.GetUniqUserID());
        // }


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