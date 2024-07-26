using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Data;
using _Game.UI._Shop.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.ShopDataProvider
{
    public class ShopDataProvider : IShopDataProvider
    {
        private const int PERSISTENT_TIMELINE_ID = -2;
        
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IAssetRegistry _assetRegistry;

        public ShopDataProvider(
            IShopConfigRepository shopConfigRepository,
            IAssetRegistry assetRegistry)
        {
            _shopConfigRepository = shopConfigRepository;
            _assetRegistry = assetRegistry;
        }


        public async UniTask<DataPool<int, ShopItemStaticData>> LoadShopData()
        {
            var configs = _shopConfigRepository.GetConfigs();
            DataPool<int, ShopItemStaticData> dataPool = new DataPool<int, ShopItemStaticData>();
            foreach (var config in configs)
            {
                Sprite majorProductIconKey = null;
                if (config.MajorProductIconKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    majorProductIconKey = await
                        _assetRegistry.LoadAsset<Sprite>(config.MajorProductIconKey, PERSISTENT_TIMELINE_ID,
                            Constants.CacheContext.GENERAL);
                }

                
                Sprite minorProductIconKey = null;
                if (config.MinorProductIconKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    minorProductIconKey = await
                        _assetRegistry.LoadAsset<Sprite>(config.MinorProductIconKey, PERSISTENT_TIMELINE_ID,
                            Constants.CacheContext.GENERAL);
                }
                
                Sprite currencyIconKey = null;
                if (config.CurrencyIconKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    currencyIconKey = await
                        _assetRegistry.LoadAsset<Sprite>(config.CurrencyIconKey, PERSISTENT_TIMELINE_ID,
                            Constants.CacheContext.GENERAL);
                }


                ShopItemStaticData data = new ShopItemStaticData()
                {
                    MajorProductIcon = majorProductIconKey,
                    MinorProductIcon = minorProductIconKey,
                    CurrencyIcon = currencyIconKey,
                };

                dataPool.Add(config.Id, data);
            }

            return dataPool;
        }
    }
}