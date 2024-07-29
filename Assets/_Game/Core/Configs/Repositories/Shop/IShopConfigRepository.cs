using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.Configs.Repositories.Shop
{
    public interface IShopConfigRepository
    {
        List<ProductConfig> GetIAPConfig();
        List<ProductConfig> GetIGPConfig();
        ProductConfig GetPlacementConfig();
        IEnumerable<ProductConfig> GetConfigs();
    }
}