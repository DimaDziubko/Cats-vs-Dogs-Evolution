using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.Analytics
{
    public class AppsFlyerAnalyticsService : IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        public AppsFlyerAnalyticsService(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;
        }

        public void Dispose()
        {

        }

        private void Init()
        {
            _gameInitializer.OnPostInitialization -= Init;
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
