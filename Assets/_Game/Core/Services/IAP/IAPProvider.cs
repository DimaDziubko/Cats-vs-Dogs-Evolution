using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories.Shop;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace _Game.Core.Services.IAP
{
    public class IAPProvider : IStoreListener
    {
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;

        private IStoreController _controller;
        private IExtensionProvider _extensions;

        private IAPService _iapService;

        public Dictionary<string, ProductConfig> Configs { get; private set; }
        public Dictionary<string, Product> Products { get; private set; }

        public event Action Initialized;

        public bool IsInitialized => _controller != null && _extensions != null;


        public IAPProvider(
            IMyLogger logger,
            IShopConfigRepository shopConfigRepository)
        {
            _logger = logger;
            _shopConfigRepository = shopConfigRepository;
        }

        public void Initialize(IAPService iapService)
        {

            _iapService = iapService;

            Configs = new Dictionary<string, ProductConfig>();
            Products = new Dictionary<string, Product>();

            Load();

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (ProductConfig productConfig in Configs.Values)
            {
                builder.AddProduct(productConfig.IAP_ID, productConfig.ProductType);
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
                Products.Add(product.definition.id, product);
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

            // Process a successful purchase after validation
            if (validPurchase && isValidID)
            {
                Product Prod = purchaseEvent.purchasedProduct;


                MadPixelAnalytics.AnalyticsManager.PaymentSucceed(Prod);
                MAXHelper.AdsManager.AddPurchaseKeyword();
            }

            return _iapService.ProcessPurchase(purchaseEvent.purchasedProduct);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
            _logger.LogError($"Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason} transaction id {product.transactionID}");

        private void Load()
        {
            Configs = _shopConfigRepository
                .GetIAPConfig()
                .ToDictionary(x => x.IAP_ID,
                x => x);
        }
    }
}