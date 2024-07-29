using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Utils;

namespace _Game.Core.Configs.Repositories.Shop
{
    public class ShopConfigRepository : IShopConfigRepository
    {
        private readonly IUserContainer _userContainer;
        
        public ShopConfigRepository(IUserContainer userContainer) =>
            _userContainer = userContainer;

        public List<ProductConfig> GetIAPConfig()
        {
            return _userContainer.GameConfig.ShopConfig.Products
                .Where(x => x.IAP_ID != Constants.ConfigKeys.MISSING_KEY && 
                            x.IAP_ID != Constants.ConfigKeys.PLACEMENT)
                .ToList();
        }
        
        public List<ProductConfig> GetIGPConfig()
        {
            return _userContainer.GameConfig.ShopConfig.Products
                .Where(x => x.IAP_ID == Constants.ConfigKeys.MISSING_KEY &&
                 x.IAP_ID != Constants.ConfigKeys.PLACEMENT) 
                .ToList();
        }
        
        public ProductConfig GetPlacementConfig()
        {
            return _userContainer.GameConfig.ShopConfig.Products
                .Find(x => x.IAP_ID == Constants.ConfigKeys.PLACEMENT);
        }

        public IEnumerable<ProductConfig> GetConfigs() => 
            _userContainer.GameConfig.ShopConfig.Products;
    }
}