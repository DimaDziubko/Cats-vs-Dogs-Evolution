using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using _Game.Core._GameMode;
using _Game.Core.Ads;
using AppsFlyerConnector;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Game.Core.Services.Analytics
{
    public class AppsFlyerAnalyticsService : IInitializable, IDisposable
    {
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly AppsFlyerSettings _settings;
        private readonly IAdsService _iAdsService;

        public AppsFlyerAnalyticsService(
            IMyLogger logger,
            AppsFlyerSettings settings,
            IAdsService iAdsService)
        {
            _logger = logger;
            _settings = settings;
            _iAdsService = iAdsService;    
        }

        void IInitializable.Initialize()
        {
            AppsFlyer.setIsDebug(_settings.IsDebug);
            
            AppsFlyer.setCustomerUserId(UnityEngine.Device.SystemInfo.deviceUniqueIdentifier);

            AppsFlyer.initSDK(_settings.DevKey, _settings.AppID, _settings);
            
            AppsFlyerPurchaseConnector.init(_settings, Store.GOOGLE);

            AppsFlyerPurchaseConnector.setIsSandbox(GameMode.I.TestMode);
            
            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            
            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(
                AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions,
                AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            
            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();
            
            AppsFlyer.startSDK();

            AppsFlyerAdRevenue.start();

            AppsFlyerAdRevenue.setIsDebug(GameMode.I.TestMode);

            _iAdsService.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

        void IDisposable.Dispose()
        {
            _iAdsService.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
        }
        
        private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Dictionary<string, string> additionalParams = new Dictionary<string, string>();
            additionalParams.Add(AFAdRevenueEvent.AD_UNIT, adInfo.AdUnitIdentifier);
            additionalParams.Add(AFAdRevenueEvent.AD_TYPE, adInfo.AdFormat);
            AppsFlyerAdRevenue.logAdRevenue(adInfo.NetworkName,
                AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                adInfo.Revenue, "USD", additionalParams);
        }
        
        /// <summary>
        /// * Event parameters for af_initiated_checkout event
        /// </summary>
        /// <param name="product"></param>
        public void InitiatedCheckout(Product product)
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>
            {
                { AFInAppEvents.PRICE, product.metadata.localizedPriceString }, // Total price in the cart
                { AFInAppEvents.CONTENT_ID, product.definition.id }, // ID of the product in the cart
                { AFInAppEvents.CONTENT_TYPE, product.definition.type.ToString() }, // Product type/category
                { AFInAppEvents.CURRENCY, product.metadata.isoCurrencyCode }, // Currency during time of checkout
                { AFInAppEvents.QUANTITY, "1" } // Assuming 1 item per purchase
            };

            AppsFlyer.sendEvent(AFInAppEvents.INITIATED_CHECKOUT, eventParameters);

            DebugEvent("INITIATED_CHECKOUT", eventParameters);
        }

        /// <summary>
        /// Event parameters for af_purchase event
        /// </summary>
        /// <param name="product"></param>
        public void EventIAPPurchase(Product product)
        {
            Dictionary<string, string> eventParameters = GetProductDictEvent(product);

            /*
             * Send af_purchase event.
             * Trigger: User lands on the thank you page after a successful purchase
             */
            AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventParameters);

            DebugEvent("PURCHASE", eventParameters);
        }


        /// <summary>
        /// Event parameters for first_purchase event
        /// </summary>
        /// <param name="product"></param>
        public void FirstIAPPurchase(Product product)
        {
            //Dictionary<string, string> eventParameters = GetProductDictEvent(product);

            ///*
            //  * Send first_purchase event.
            //  * Trigger: User completes their first purchase
            //  */
            //AppsFlyer.sendEvent("first_purchase", eventParameters);

            //DebugEvent("first_purchase", eventParameters);
        }

        private Dictionary<string, string> GetProductDictEvent(Product product)
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>
            {
                { AFInAppEvents.REVENUE, product.metadata.localizedPrice.ToString() }, // Estimated revenue from the purchase
                { AFInAppEvents.PRICE, product.metadata.localizedPriceString }, // Overall purchase sum
                { "af_content", product.definition.id }, // International Article Number (EAN) or product/content identifier
                { AFInAppEvents.CONTENT_ID, product.definition.id }, // Item ID
                { AFInAppEvents.CONTENT_TYPE, product.definition.type.ToString() }, // Item category
                { AFInAppEvents.CURRENCY, product.metadata.isoCurrencyCode }, // Currency code
                { AFInAppEvents.QUANTITY, "1" }, // Number of items in the cart (assuming 1 item per purchase)
                { "af_order_id", product.transactionID }, // ID of the order generated after the purchase
                { AFInAppEvents.RECEIPT_ID, product.receipt } // Order ID
            };
            return eventParameters;
        }

        public void DebugEvent(string eventName, Dictionary<string, string> eventParameters)
        {
            Debug.Log("AppsFlyer EVENT : " + eventName);
            foreach (var kvp in eventParameters)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }
            Debug.Log("AppsFlyer EVENT DONE : " + eventName);
        }
    }
}
