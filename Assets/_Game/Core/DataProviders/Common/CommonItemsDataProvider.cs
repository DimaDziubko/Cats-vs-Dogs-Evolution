using System;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Facade;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Common
{
    public class CommonItemsDataProvider : ICommonItemsDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public CommonItemsDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        public async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(
            ICommonItemsConfigRepository itemsConfigRepository, 
            LoadContext context)
        {
            DataPool<Race, Sprite> pool = new DataPool<Race, Sprite>();
            
            foreach (Race race in Enum.GetValues(typeof(Race)))
            {
                var iconKey = itemsConfigRepository.GetFoodIconKey(race);
                var iconSprite = await _assetRegistry.LoadAsset<Sprite>(iconKey, context.Timeline, context.CacheContext);
                pool.Add(race, iconSprite);
                
                _logger.Log($"Food icon for race{race} loaded successfully");
            }

            return  pool;
        }

        public async UniTask<Sprite> LoadBaseIcon(ICommonItemsConfigRepository itemsConfigRepository, LoadContext context)
        {
            var iconKey = itemsConfigRepository.GetTowerIconKey();
            var iconSprite = await _assetRegistry.LoadAsset<Sprite>(iconKey, context.Timeline, context.CacheContext);
            _logger.Log($"Base icon loaded successfully");
            return iconSprite;
        }
    }
}