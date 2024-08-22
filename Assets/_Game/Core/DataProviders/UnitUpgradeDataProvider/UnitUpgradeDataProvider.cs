using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
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
                await _assetRegistry.Warmup<IList<Sprite>>(config.CatIconAtlas);
                await _assetRegistry.Warmup<IList<Sprite>>(config.DogIconAtlas);
                
                string iconName = config.GetUnitIconNameForRace(context.Race);
                IList<Sprite> atlas = await LoadIconAtlasForRace(config, context);
                Sprite icon = atlas.FirstOrDefault(x => x.name == iconName);
                
                _logger.Log($"Icon with name {iconName} loaded");
                
                if (icon == null)
                {
                    _logger.LogWarning($"Icon with name {iconName} not found in atlas for unit type {config.Type}");
                    continue;
                }

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

        private async UniTask<IList<Sprite>> LoadIconAtlasForRace(WarriorConfig config, LoadContext context)
        {
            switch (context.Race)
            {
                case Race.Cat:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, context.Timeline, context.CacheContext);
                case Race.Dog:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.DogIconAtlas, context.Timeline, context.CacheContext);
                case Race.None:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, context.Timeline, context.CacheContext);
                default:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, context.Timeline, context.CacheContext);
            }
        }
    }
}