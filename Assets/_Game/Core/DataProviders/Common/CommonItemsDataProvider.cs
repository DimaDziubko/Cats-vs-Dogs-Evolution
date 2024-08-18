﻿using System;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Common
{
    public class CommonItemsDataProvider : ICommonItemsDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private readonly ICommonItemsConfigRepository _commonItemsConfigRepository;
        
        public CommonItemsDataProvider(
            IAssetRegistry assetRegistry,
            ICommonItemsConfigRepository commonItemsConfigRepository,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _commonItemsConfigRepository = commonItemsConfigRepository;
            _logger = logger;
        }
        public async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(LoadContext context)
        {
            DataPool<Race, Sprite> pool = new DataPool<Race, Sprite>();
            
            foreach (Race race in Enum.GetValues(typeof(Race)))
            {
                var iconReference = _commonItemsConfigRepository.GetFoodIconKey(race);

                var iconSprite = await _assetRegistry.LoadAsset<Sprite>(iconReference, context.Timeline, context.CacheContext);
                pool.Add(race, iconSprite);
                
                _logger.Log($"Food icon {iconSprite.name} for race{race} loaded successfully");
            }

            return  pool;
        }
        
        public async UniTask<Sprite> LoadBaseIcon(LoadContext context)
        {
            string key = _commonItemsConfigRepository.GetBaseIconKey();
            var iconSprite = await _assetRegistry.LoadAsset<Sprite>(key, context.Timeline, context.CacheContext);
            _logger.Log($"Base icon {iconSprite.name} loaded successfully");
            return iconSprite;
        }
    }
}