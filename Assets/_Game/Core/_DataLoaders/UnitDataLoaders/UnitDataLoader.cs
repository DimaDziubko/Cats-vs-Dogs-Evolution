using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.UnitDataLoaders
{
    public class UnitDataLoader : IUnitDataLoader
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public UnitDataLoader(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _assetRegistry = assetRegistry;
        }

        public async UniTask<IUnitData> LoadUnitDataAsync(UnitLoadOptions options)
        {
            Race race;
            switch (options.Faction)
            {
                case Faction.Player:
                    race = options.CurrentRace;
                    break;
                case Faction.Enemy:
                    race = options.CurrentRace == Race.Cat ? Race.Dog : Race.Cat;
                    break;
                default:
                    race = options.CurrentRace;
                    break;
            }

            _logger.Log($"Unit with id {options.Config.Id} load successfully");
            
            
            await _assetRegistry.Warmup<IList<Sprite>>(options.Config.CatIconAtlas);
            await _assetRegistry.Warmup<IList<Sprite>>(options.Config.DogIconAtlas);
                
            string warriorIconName = options.Config.GetUnitIconNameForRace(race);

            IList<Sprite> iconAtlas = await LoadIconAtlasForRace(race, options.Config, options.Timeline, options.CacheContext);

            Sprite icon = iconAtlas.FirstOrDefault(x => x.name == warriorIconName);

            return new UnitData(options.Config)
            {
                Race = race,
                UnitLayer = options.Config.GetUnitLayerForFaction(options.Faction),
                AggroLayer = options.Config.GetAggroLayerForFaction(options.Faction),
                AttackLayer = options.Config.GetAttackLayerForFaction(options.Faction),
                Icon = icon,
            };
        }
        
        private async UniTask<IList<Sprite>> LoadIconAtlasForRace(Race race, WarriorConfig config, int timeline, int cacheContext)
        {
            switch (race)
            {
                case Race.Cat:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, cacheContext);
                case Race.Dog:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.DogIconAtlas, timeline, cacheContext);
                case Race.None:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, cacheContext);
                default:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, cacheContext);
            }
        }
    }
}