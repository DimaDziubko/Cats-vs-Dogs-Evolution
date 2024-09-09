using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.DataProviders.UnitUpgradeDataProvider;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.UnitUpgradeDataProvider
{
    public class UnitUpgradeDataLoader : IUnitUpgradeDataLoader
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public UnitUpgradeDataLoader(
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
                
                string playerIconName = config.GetUnitIconNameForRace(context.Race);
                string enemyIconName = config.GetUnitIconNameForRace(context.Race.OppositeRace());
                
                IList<Sprite> yourIconsAtlas = await LoadIconAtlasForRace(context.Race, config, context);
                IList<Sprite> enemyIconsAtlas = await LoadIconAtlasForRace(context.Race.OppositeRace(), config, context);
                
                Sprite yourIcon = yourIconsAtlas.FirstOrDefault(x => x.name == playerIconName);
                Sprite enemyIcon = enemyIconsAtlas.FirstOrDefault(x => x.name == enemyIconName);
                
                _logger.Log($"Icon with name {playerIconName} loaded");
                
                if (yourIcon != null)
                {
                    var item = new UnitUpgradeItemStaticData
                    {
                        Type = config.Type,
                        WarriorIcon = yourIcon,
                        Name = config.Name,
                        Price = config.Price,
                    };

                    _logger.Log($"Unit upgrade item with type {config.Type} loaded successfully");
                    pool.Add(config.Type, item);
                    
                    continue;
                }
                _logger.LogWarning($"Icon with name {playerIconName} not found in atlas for unit type {config.Type}");
            }

            return pool;
        }

        private async UniTask<IList<Sprite>> LoadIconAtlasForRace(Race race, WarriorConfig config, LoadContext context)
        {
            switch (race)
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