using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using Assets._Game.Core.UserState;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPService : IIAPService, IDisposable
    {
        public event Action Initialized;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAPProvider _iapProvider;

        private IPurchaseDataStateReadonly PurchaseData => _userContainer.State.PurchaseDataState;
        private IBattleSpeedStateReadonly BattleSpeedStateReadonly => _userContainer.State.BattleSpeedState;
        
        public bool IsInitialized => _iapProvider.IsInitialized;

        public IAPService(
            IAPProvider iapProvider, 
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _iapProvider = iapProvider;
            _gameInitializer = gameInitializer;
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
                
                if (ProductBoughtOut(boughtIap, config))
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
            if (configItemType == ItemType.x2 && BattleSpeedStateReadonly.PermanentSpeedId < 1)
            {
                return false;
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

            // switch (productConfig.ItemType)
            // {
            //     case ItemType.Skull:
            //         //_progressService.Progress.WorldData.LootData.Add(productConfig.Quantity);
            //         //_progressService.Progress.PurchaseData.AddPurchase(purchasedProduct.definition.id);
            //         break;
            // }

            return PurchaseProcessingResult.Complete;
        }
    }
}