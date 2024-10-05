using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.IAP.Data;
using DevToDev.AntiCheat;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace _Game.Core.Services.IAP
{
    public class IAPProvider : IStoreListener
    {
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;

        public event Action<Product> OnTrackPurchaseDev2Dev; //That Action just for Dev2Dev Because AntiCheat


        private IStoreController _controller;
        private IExtensionProvider _extensions;

        private IAPService _iapService;

        public Dictionary<string, GemsBundleConfig> GemsBundleConfigs { get; private set; }
        public Dictionary<string, Product> GemsBundleProducts { get; private set; }


        public Dictionary<string, SpeedBoostOfferConfig> SpeedBoostOfferConfigs { get; private set; }
        public Dictionary<string, Product> SpeedOffers { get; private set; }


        public Dictionary<string, ProfitOfferConfig> ProfitOfferConfigs { get; private set; }
        public Dictionary<string, Product> ProfitOffers { get; set; }

        public event Action Initialized;
        public bool IsInitialized => _controller != null && _extensions != null;


        public IAPProvider(
            IMyLogger logger,
            IConfigRepositoryFacade configRepositoryFacade)
        {
            _logger = logger;
            _shopConfigRepository = configRepositoryFacade.ShopConfigRepository;
        }

        public void Initialize(IAPService iapService)
        {
            _iapService = iapService;

            GemsBundleConfigs = new Dictionary<string, GemsBundleConfig>();
            GemsBundleProducts = new Dictionary<string, Product>();

            SpeedBoostOfferConfigs = new Dictionary<string, SpeedBoostOfferConfig>();
            SpeedOffers = new Dictionary<string, Product>();

            ProfitOfferConfigs = new Dictionary<string, ProfitOfferConfig>();
            ProfitOffers = new Dictionary<string, Product>();

            Load();

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (GemsBundleConfig gemsBundleConfig in GemsBundleConfigs.Values)
            {
                builder.AddProduct(gemsBundleConfig.IAP_ID, gemsBundleConfig.ProductType);
            }

            foreach (SpeedBoostOfferConfig speedBoostOfferConfig in SpeedBoostOfferConfigs.Values)
            {
                builder.AddProduct(speedBoostOfferConfig.IAP_ID, speedBoostOfferConfig.ProductType);
            }

            foreach (ProfitOfferConfig profitOfferConfig in ProfitOfferConfigs.Values)
            {
                builder.AddProduct(profitOfferConfig.IAP_ID, profitOfferConfig.ProductType);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void StartPurchase(string productId) =>
            _controller.InitiatePurchase(productId);

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _extensions = extensions;

            foreach (Product product in _controller.products.all)
            {
                if (GemsBundleConfigs.ContainsKey(product.definition.id))
                {
                    GemsBundleProducts.Add(product.definition.id, product);
                }

                if (SpeedBoostOfferConfigs.ContainsKey(product.definition.id))
                {
                    SpeedOffers.Add(product.definition.id, product);
                }

                if (ProfitOfferConfigs.ContainsKey(product.definition.id))
                {
                    ProfitOffers.Add(product.definition.id, product);
                }
            }

            Initialized?.Invoke();

            _logger.Log("UnityPurchasing initialization success");
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            _logger.LogError($"UnityPurchasing initialization failed {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message) =>
            _logger.LogError($"UnityPurchasing initialization failed {error}, {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            _logger.Log($"UnityPurchasing ProcessPurchase success {purchaseEvent.purchasedProduct.definition.id}");

            List<string> receitsIDs = new List<string>();

            bool validPurchase = true;

#if !UNITY_EDITOR
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.

            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                //Debug.Log("Valid!");
                foreach (IPurchaseReceipt productReceipt in result) {
                    receitsIDs.Add(productReceipt.productID);
                }
            } catch (IAPSecurityException) {
                //Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }

#endif


            bool isValidID = false;
            if (receitsIDs != null && receitsIDs.Count > 0)
            {
                foreach (string ProductID in receitsIDs)
                {
                    //Debug.Log($"{E.purchasedProduct.definition.storeSpecificId}, and {ProductID}");
                    if (purchaseEvent.purchasedProduct.definition.storeSpecificId.Equals(ProductID))
                    {
                        isValidID = true;
                        break;
                    }
                }
            }

#if UNITY_EDITOR
            validPurchase = isValidID = true;
#endif


            if (validPurchase && isValidID)
            {
                Product prod = purchaseEvent.purchasedProduct;
                if (prod != null)
                {
                    string receipt = prod.receipt;
                    string currency = prod.metadata.isoCurrencyCode;
                    int amount = decimal.ToInt32(prod.metadata.localizedPrice * 100);
#if !UNITY_EDITOR
#if UNITY_ANDROID
                    //GA
                    ReceiptData receiptAndroid = JsonUtility.FromJson<ReceiptData>(receipt);
                    PayloadAndroidData receiptPayload = JsonUtility.FromJson<PayloadAndroidData>(receiptAndroid.Payload);
                    GameAnalytics.NewBusinessEventGooglePlay(currency, amount, prod.definition.payout.subtype,
                        purchaseEvent.purchasedProduct.definition.id, "Shop", receiptPayload.json, receiptPayload.signature);

                    DTDAntiCheat.VerifyPayment(_iapService.PublicGooglePlayKey, purchaseEvent.purchasedProduct.receipt, result =>
                        {
                            if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                                result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptInternalError ||
                                result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptServerError)
                            {
                                // Code for valid result.
                                OnTrackPurchaseDev2Dev?.Invoke(prod);
                            }
                            else
                            {
                                // Code for invalid result.
                            }
                        });
#endif
#if UNITY_IPHONE
                    //GA
                    ReceiptData receiptiOS = JsonUtility.FromJson<ReceiptData>(receipt);
                    string receiptPayload = receiptiOS.Payload;
                    GameAnalytics.NewBusinessEventIOS(currency, amount, prod.definition.payout.subtype,
                            purchaseEvent.purchasedProduct.definition.id, "Shop", receiptPayload);

                    DTDAntiCheat.VerifyPayment(purchaseEvent.purchasedProduct.receipt, result =>
                    {
                        if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                            result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptInternalError ||
                            result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptServerError)
                        {
                            OnTrackPurchaseDev2Dev?.Invoke(prod);
                        }
                        else
                        {
                        }
                    });
#endif
#endif
                }

            }
            return _iapService.ProcessPurchase(purchaseEvent.purchasedProduct);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
            _logger.LogError($"Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason} transaction id {product.transactionID}");

        private void Load()
        {
            GemsBundleConfigs = _shopConfigRepository
                .GetGemsBundleConfigs()
                .ToDictionary(x => x.IAP_ID,
                x => x);

            SpeedBoostOfferConfigs = _shopConfigRepository
                .GetSpeedBoostOfferConfigs()
                .ToDictionary(x => x.IAP_ID,
                    x => x);

            ProfitOfferConfigs = _shopConfigRepository
                .GetProfitOfferConfigs()
                .ToDictionary(x => x.IAP_ID,
                x => x);
        }
    }
}