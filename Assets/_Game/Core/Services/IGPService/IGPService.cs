using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;
using _Game.Utils;
using Assets._Game.Core.UserState;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core.Services.IGPService
{
    public class IGPDto
    {
        public string PurchaseId;
        public string PurchaseType;
        public int PurchaseAmount;
        public int PurchasePrice;
        public string PurchaseCurrency;
        public Dictionary<string, int> Resources;
    }
    
    public class IGPService : IIGPService
    {
        public event Action<IGPDto> Purchased;
        
        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public IGPService(
            IUserContainer userContainer,
            IShopConfigRepository shopConfigRepository)
        {
            _userContainer = userContainer;
            _shopConfigRepository = shopConfigRepository;
        }

        public List<ProductDescription> Products() => 
            ProductDefinitions().ToList();

        public void StartPurchase(ProductConfig config)
        {
            switch (config.ItemType)
            {
                case ItemType.x1_5:
                    break;
                case ItemType.x2:
                    break;
                case ItemType.Gems:
                    break;
                case ItemType.Coins:
                    _userContainer.PurchaseStateHandler
                        .PurchaseCoinsWithGems(config.Quantity, config.Price, CurrenciesSource.Shop);
                    Notify(
                        config.IGP_ID,
                        config.ItemType.ToString(),
                        (int)config.Quantity,
                        config.Price,
                        Currencies.Gems.ToString());
                    break;
            }
        }

        private void Notify(
            string purchaseId,
            string purchaseType,
            int purchaseAmount,
            int purchasePrice,
            string purchaseCurrency,
            Dictionary<string, int> resources = null
            )
        {
            var dto = new IGPDto()
            {
                PurchaseId = purchaseId,
                PurchaseType = purchaseType,
                PurchaseAmount = purchaseAmount,
                PurchasePrice = purchasePrice,
                PurchaseCurrency = purchaseCurrency,
                Resources = resources
            };
            Purchased?.Invoke(dto);
        }


        private IEnumerable<ProductDescription> ProductDefinitions()
        {
            var configs = _shopConfigRepository.GetIGPConfig();

            foreach (var config in configs)
            {
                if (config.ItemType == ItemType.Coins)
                {
                    var level = Mathf.Max(TimelineState.BaseHealthLevel, TimelineState.FoodProductionLevel);
                    config.Quantity = config.QuantityExponential.GetValue(level);
                }
                
                yield return new ProductDescription()
                {
                    Id = Constants.ConfigKeys.MISSING_KEY,
                    Config = config,
                    Product = null,
                    AvailablePurchasesLeft = config.MaxPurchaseCount,
                
                };
            }
        }
    }
}