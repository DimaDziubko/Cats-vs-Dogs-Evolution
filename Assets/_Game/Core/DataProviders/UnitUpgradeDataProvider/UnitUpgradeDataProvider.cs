﻿using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.UnitUpgradeDataProvider
{
    public class UnitUpgradeDataProvider : IUnitUpgradeDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public UnitUpgradeDataProvider(
            IMyLogger logger,
            IAssetRegistry assetRegistry)
        {
            _logger = logger;
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            DataPool<UnitType, UnitUpgradeItemStaticData> pool = new DataPool<UnitType, UnitUpgradeItemStaticData>();
                
            foreach (var config in configs)
            {
                string iconKey = config.GetUnitIconKeyForRace(context.Race);
                
                Sprite icon = await _assetRegistry.LoadAsset<Sprite>(iconKey, context.Timeline, context.CacheContext);

                var item = new UnitUpgradeItemStaticData
                {
                    Type = config.Type,
                    Icon = icon, 
                    Name = config.Name,
                    Price = config.Price,
                };
                
                _logger.Log($"Unit upgrade item with type {config.Type} loaded successfully");
                
                pool.Add(config.Type, item);
            }

            return pool;
        }
    }
}