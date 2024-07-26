using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Utils;

namespace _Game.Core.Services.IGPService
{
    public class IGPService : IIGPService
    {
        private IUserContainer _userContainer;
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
                    _userContainer.PurchaseCoinsWithGems(productDescriptionConfig.Quantity, productDescriptionConfig.Price);
                    break;
            }
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
                    AvailablePurchasesLeft = -1,
                
                };
            }
        }
    }
}