using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.CardsDataProvider
{
    public class CardsDataProvider : ICardsDataProvider
    {
        private const int PERSISTENT_TIMELINE_ID = -2;

        private readonly ICardsConfigRepository _shopConfigRepository;
        private readonly IAssetRegistry _assetRegistry;

        public CardsDataProvider(
            IConfigRepositoryFacade configRepositoryFacade,
            IAssetRegistry assetRegistry)
        {
            _shopConfigRepository = configRepositoryFacade.CardsConfigRepository;
            _assetRegistry = assetRegistry;
        }


        public async UniTask<DataPool<int, Sprite>> LoadCardIcons()
        {
            // var configs = _cardsConfiRepository.GetConfigs();
            // DataPool<int, ShopItemStaticData> dataPool = new DataPool<int, ShopItemStaticData>();
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
            //     ShopItemStaticData data = new ShopItemStaticData()
            //     {
            //         MajorProductIcon = majorProductIconKey,
            //         MinorProductIcon = minorProductIconKey,
            //         CurrencyIcon = currencyIconKey,
            //     };
            //
            //     dataPool.Add(config.Id, data);
            // }

            return null;
        }
    }

}