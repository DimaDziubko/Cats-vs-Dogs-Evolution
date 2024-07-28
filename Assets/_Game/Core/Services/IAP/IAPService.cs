using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.UserState;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPService : IIAPService, IDisposable
    {
        private const int INFINITY_PURCHASES_TRIGGER = -1;
        
        public event Action Initialized;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAPProvider _iapProvider;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private IPurchaseDataStateReadonly PurchaseData => _userContainer.State.PurchaseDataState;
        private IBattleSpeedStateReadonly BattleSpeedStateReadonly => _userContainer.State.BattleSpeedState;
        
        public bool IsInitialized => _iapProvider.IsInitialized;

        
        public IAPService(
            IAPProvider iapProvider, 
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _userContainer = userContainer;
            _iapProvider = iapProvider;
            _gameInitializer = gameInitializer;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer.OnPreInitialization += Init;
        }

        private void Init()
        {
            _iapProvider.Initialize(this);
            _iapProvider.Initialized += () => Initialized?.Invoke();
        }

        public void Dispose() => 
            _gameInitializer.OnPreInitialization -= Init;
                                      
        public List<ProductDescription> Products() => 
            ProductDefinitions().ToList();


        private IEnumerable<ProductDescription> ProductDefinitions()
        {
            foreach (string productId in _iapProvider.Products.Keys)
            {
                ProductConfig config = _iapProvider.Configs[productId];
                Product product = _iapProvider.Products[productId];

                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == productId);
                
                if (
                    ProductBoughtOut(boughtIap, config) && 
                    config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER)
                {
                    continue;
                }

                if (!IsAvailable(config.ItemType))
                {
                    continue;
                }
                
                yield return new ProductDescription()
                {
                    Id = productId,
                    Config = config,
                    Product = product,
                    AvailablePurchasesLeft = boughtIap != null
                        ? config.MaxPurchaseCount - boughtIap.Count
                        : config.MaxPurchaseCount,
                
                };
            }
        }

        private bool IsAvailable(ItemType configItemType)
        {
            switch (configItemType)
            {
                case ItemType.x1_5:
                    return IsSpeedItemAvailable(configItemType);
                case ItemType.x2:
                    return IsSpeedItemAvailable(configItemType);
                case ItemType.Coins:
                    return true;
                case ItemType.Gems:
                    return true;
                default:
                    return true;
            }
        }

        private bool IsSpeedItemAvailable(ItemType configItemType)
        {
            bool isSpeedFeatureUnlocked = _featureUnlockSystem.IsFeatureUnlocked(Feature.BattleSpeed);
    
            if (!isSpeedFeatureUnlocked)
            {
                return false;
            }
            
            if (configItemType == ItemType.x1_5)
            {
                return true;
            }
            
            if (configItemType == ItemType.x2)
            {
                bool isX15Bought = PurchaseData.BoughtIAPs
                    .Any(x => x.IAPId == "com.catsvsdogs.speedx1" && x.Count > 0);
                return isX15Bought;
            }

            return true;
        }
        
        
        private bool ProductBoughtOut(BoughtIAP boughtIap, ProductConfig config) => 
            boughtIap != null && boughtIap.Count >= config.MaxPurchaseCount;


        public void StartPurchase(string productId) => 
            _iapProvider.StartPurchase(productId);

        public PurchaseProcessingResult ProcessPurchase(Product purchasedProduct)
        {
            ProductConfig productConfig =  _iapProvider.Configs[purchasedProduct.definition.id];

            switch (productConfig.ItemType)
            {
                case ItemType.x1_5:
                    //TODO Check later
                    int speedIdFor1_5 = 1; 
                    _userContainer.ChangePermanentSpeedId(speedIdFor1_5);
                    _userContainer.AddPurchase(purchasedProduct.definition.id);
                    break;
                case ItemType.x2:
                    //TODO Check later
                    int speedIdFor2 = 2; 
                    _userContainer.ChangePermanentSpeedId(speedIdFor2);
                    _userContainer.AddPurchase(purchasedProduct.definition.id);
                    break;
                case ItemType.Coins:
                    break;
                case ItemType.Gems:
                    _userContainer.AddGems(productConfig.Quantity);
                    _userContainer.AddPurchase(purchasedProduct.definition.id);
                    break;
            }

            return PurchaseProcessingResult.Complete;
        }
    }
}