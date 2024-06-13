using System;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Common
{
    public class CommonItemsDataProvider : ICommonItemsDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public CommonItemsDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }
        public async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, int context)
        {
            DataPool<Race, Sprite> pool = new DataPool<Race, Sprite>();
            
            foreach (Race race in Enum.GetValues(typeof(Race)))
            {
                var iconKey = itemsConfigRepository.GetFoodIconKey(race);
                var iconSprite = await _assetRegistry.LoadAsset<Sprite>(iconKey, context);
                pool.Add(race, iconSprite);
            }

            return  pool;
        }

        public async UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository, int context)
        {
            var iconKey = itemsConfigRepository.GetTowerIconKey();
            var iconSprite = await _assetRegistry.LoadAsset<Sprite>(iconKey, context);
            return iconSprite;
        }
    }
}