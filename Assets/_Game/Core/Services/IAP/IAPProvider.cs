using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPProvider : IStoreListener
    {
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;

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

            return _iapService.ProcessPurchase(purchaseEvent.purchasedProduct);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) => 
            _logger.LogError($"Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason} transaction id {product.transactionID}");

        private void Load()
        {
            GemsBundleConfigs = _shopConfigRepository
                .GetGemsBundleConfigs()
                .ToDictionary(x =>x.IAP_ID, 
                x=>x);
            
            SpeedBoostOfferConfigs = _shopConfigRepository
                .GetSpeedBoostOfferConfigs()
                .ToDictionary(x =>x.IAP_ID, 
                    x=>x);

            ProfitOfferConfigs = _shopConfigRepository
                .GetProfitOfferConfigs()
                .ToDictionary(x => x.IAP_ID,
                x => x);
        }
    }
}