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
        private Product _productTryToPurchase;

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
            if (purchaseEvent == null)
            {
                Debug.LogError("PurchaseEventArgs is null.");
                // Handle the error or return an appropriate result
                return PurchaseProcessingResult.Complete;
            }

            if (purchaseEvent.purchasedProduct == null)
            {
                Debug.LogError("PurchasedProduct is null.");
                return PurchaseProcessingResult.Complete;
            }

            if (purchaseEvent.purchasedProduct.definition == null)
            {
                Debug.LogError("Product definition is null.");
                return PurchaseProcessingResult.Complete;
            }

            if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.definition.id))
            {
                Debug.LogError("Product definition ID is null or empty.");
                return PurchaseProcessingResult.Complete;
            }

            Debug.Log($"UnityPurchasing ProcessPurchase success {purchaseEvent.purchasedProduct.definition.id}");

            _productTryToPurchase = purchaseEvent.purchasedProduct;

            List<string> receitsIDs = new List<string>();

            bool validPurchase = true;

            //#if !UNITY_EDITOR
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try
            {
                if (string.IsNullOrEmpty(_productTryToPurchase.receipt))
                {
                    Debug.LogError("Product receipt is null or empty.");
                    validPurchase = false;
                }
                else
                {
                    var result = validator.Validate(_productTryToPurchase.receipt);
                    foreach (IPurchaseReceipt productReceipt in result)
                    {
                        receitsIDs.Add(productReceipt.productID);
                    }
                }
            }
            catch (IAPSecurityException ex)
            {
                Debug.LogError("Invalid receipt, not unlocking content: " + ex.Message);
                validPurchase = false;
            }
            //#endif


            bool isValidID = false;
            if (receitsIDs != null && receitsIDs.Count > 0)
            {
                foreach (string ProductID in receitsIDs)
                {
                    if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.definition.storeSpecificId))
                    {
                        Debug.LogError("Store specific ID is null or empty.");
                        continue;
                    }

                    if (string.IsNullOrEmpty(ProductID))
                    {
                        Debug.LogError("ProductID in receitsIDs is null or empty.");
                        continue;
                    }

                    if (purchaseEvent.purchasedProduct.definition.storeSpecificId.Equals(ProductID))
                    {
                        isValidID = true;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("receitsIDs is null or empty.");
            }

            //#if UNITY_EDITOR
            //            validPurchase = isValidID = true;
            //#endif


            //            if (validPurchase && isValidID)
            //            {
            //                if (_productTryToPurchase != null)
            //                {
            //                    string receipt = _productTryToPurchase.receipt;
            //                    string currency = _productTryToPurchase.metadata.isoCurrencyCode;
            //                    int amount = decimal.ToInt32(_productTryToPurchase.metadata.localizedPrice * 100);
            //#if !UNITY_EDITOR
            //#if UNITY_ANDROID
            //                    //GA
            //                    ReceiptData receiptAndroid = JsonUtility.FromJson<ReceiptData>(receipt);
            //                    PayloadAndroidData receiptPayload = JsonUtility.FromJson<PayloadAndroidData>(receiptAndroid.Payload);
            //                    GameAnalytics.NewBusinessEventGooglePlay(currency, amount, _productTryToPurchase.definition.payout.subtype,
            //                        purchaseEvent.purchasedProduct.definition.id, "Shop", receiptPayload.json, receiptPayload.signature);

            //#endif
            //#if UNITY_IPHONE
            //                    //GA
            //                    ReceiptData receiptiOS = JsonUtility.FromJson<ReceiptData>(receipt);
            //                    string receiptPayload = receiptiOS.Payload;
            //                    GameAnalytics.NewBusinessEventIOS(currency, amount, _productTryToPurchase.definition.payout.subtype,
            //                            purchaseEvent.purchasedProduct.definition.id, "Shop", receiptPayload);

            //#endif
            //#endif
            //                }
            //            }

            Debug.Log("UnityPurchasing _iapService.ProcessPurchase(_productTryToPurchase)");
            return _iapService.ProcessPurchase(_productTryToPurchase);
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