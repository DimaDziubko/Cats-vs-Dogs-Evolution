using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Data;
using _Game.Core.DataProviders.ShopDataProvider;
using _Game.UI._Shop.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.ShopDataLoader
{
    public class ShopDataLoader : IShopDataLoader
    {
        private const int PERSISTENT_TIMELINE_ID = -2;
        
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IAssetRegistry _assetRegistry;

        public ShopDataLoader(
            IConfigRepositoryFacade configRepositoryFacade,
            IAssetRegistry assetRegistry)
        {
            _shopConfigRepository = configRepositoryFacade.ShopConfigRepository;
            _assetRegistry = assetRegistry;
        }


        public async UniTask<DataPool<int, ShopItemStaticData>> LoadShopData()
        {
            //var configs = _shopConfigRepository.GetConfigs();
            DataPool<int, ShopItemStaticData> dataPool = new DataPool<int, ShopItemStaticData>();
            // foreach (var config in configs)
            // {
            //     Sprite majorProductIconKey = null;
            //     if (config.MajorProductIconKey != Constants.ConfigKeys.MISSING_KEY)
            //     {
            //         await _assetRegistry.Warmup<Sprite>(config.MajorProductIconKey);
            //         majorProductIconKey = await
            //             _assetRegistry.LoadAsset<Sprite>(config.MajorProductIconKey, PERSISTENT_TIMELINE_ID,
            //                 Constants.CacheContext.GENERAL);
            //     }
            //
            //     
            //     Sprite minorProductIconKey = null;
            //     if (config.MinorProductIconKey != Constants.ConfigKeys.MISSING_KEY)
            //     {
            //         await _assetRegistry.Warmup<Sprite>(config.MinorProductIconKey);
            //         minorProductIconKey = await
            //             _assetRegistry.LoadAsset<Sprite>(config.MinorProductIconKey, PERSISTENT_TIMELINE_ID,
            //                 Constants.CacheContext.GENERAL);
            //     }
            //     
            //     Sprite currencyIconKey = null;
            //     if (config.CurrencyIconKey != Constants.ConfigKeys.MISSING_KEY)
            //     {
            //         await _assetRegistry.Warmup<Sprite>(config.CurrencyIconKey);
            //         currencyIconKey = await
            //             _assetRegistry.LoadAsset<Sprite>(config.CurrencyIconKey, PERSISTENT_TIMELINE_ID,
            //                 Constants.CacheContext.GENERAL);
            //     }
            //
            //
            //     ShopItemStaticData data = new ShopItemStaticData()
            //     {
            //         MajorProductIcon = majorProductIconKey,
            //         MinorProductIcon = minorProductIconKey,
            //         CurrencyIcon = currencyIconKey,
            //     };
            //
            //     dataPool.Add(config.Id, data);
            // }

            return dataPool;
        }
    }
}