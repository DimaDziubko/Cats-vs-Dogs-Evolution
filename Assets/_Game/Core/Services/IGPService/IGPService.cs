using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;
using _Game.Utils;

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
        private Dictionary<ItemType, string> _purchaseIds = new Dictionary<ItemType, string>()
        {
            {ItemType.Coins, "coins"}
        };
        
        public event Action<IGPDto> Purchased;
        
        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;

        public IGPService(
            IUserContainer userContainer,
            IShopConfigRepository shopConfigRepository)
        {
            _userContainer = userContainer;
            _shopConfigRepository = shopConfigRepository;
        }

        public List<ProductDescription> Products() => 
            ProductDefinitions().ToList();

        public void StartPurchase(ProductConfig productDescriptionConfig)
        {
            switch (productDescriptionConfig.ItemType)
            {
                case ItemType.x1_5:
                    break;
                case ItemType.x2:
                    break;
                case ItemType.Gems:
                    break;
                case ItemType.Coins:
                    _userContainer.PurchaseStateHandler
                        .PurchaseCoinsWithGems(productDescriptionConfig.Quantity, productDescriptionConfig.Price, CurrenciesSource.Shop);
                    Notify(
                        _purchaseIds[productDescriptionConfig.ItemType],
                        productDescriptionConfig.ItemType.ToString(),
                        productDescriptionConfig.Quantity,
                        productDescriptionConfig.Price,
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