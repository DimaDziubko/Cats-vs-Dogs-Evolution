using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using _Game.Utils.Extensions;
using Assets._Game.Core.Data;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.Utils;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.UnitBuilderDataProvider
{
    public class UnitBuilderDataProvider : IUnitBuilderDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public UnitBuilderDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            DataPool<UnitType, UnitBuilderBtnStaticData> dataPool = new DataPool<UnitType, UnitBuilderBtnStaticData>();

            foreach (var config in configs)
            {
                var builderLoadOptions = new BuilderLoadOptions()
                {
                    Config = config,
                    CurrentRace = context.Race,
                };
                
                UnitBuilderBtnStaticData staticData = await LoadData(builderLoadOptions);
                
                _logger.Log($"Unit builder data with id {config.Id} load successfully");
                
                dataPool.Add(config.Type, staticData);
            }

            return dataPool;
        }

        private async UniTask<UnitBuilderBtnStaticData> LoadData(BuilderLoadOptions options)
        {
            await _assetRegistry.Warmup<IList<Sprite>>(options.Config.CatIconAtlas);
            await _assetRegistry.Warmup<IList<Sprite>>(options.Config.DogIconAtlas);
            
            IList<Sprite> atlas = await LoadIconAtlasForRace(options.Config, options.CurrentRace, options.Timeline);
            string iconName = options.Config.GetUnitIconNameForRace(options.CurrentRace);
            Sprite unitIcon = atlas.FirstOrDefault(sprite => sprite.name == iconName);

            if (unitIcon == null)
            {
                _logger.LogWarning($"Icon with name {iconName} not found in atlas for unit type {options.Config.Type}");
            }
            else
            {
                _logger.Log($"Unit builder data with id {options.Config.Id} loaded successfully");
            }

            return new UnitBuilderBtnStaticData
            {
                Type = options.Config.Type,
                UnitIcon = unitIcon,
                FoodPrice = options.Config.FoodPrice,
            };
        }

        private async UniTask<IList<Sprite>> LoadIconAtlasForRace(WarriorConfig config, Race race, int timeline)
        {
            switch (race)
            {
                case Race.Cat:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, Constants.CacheContext.AGE);
                case Race.Dog:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.DogIconAtlas, timeline, Constants.CacheContext.AGE);
                case Race.None:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, Constants.CacheContext.AGE);
                default:
                    return await _assetRegistry.LoadAsset<IList<Sprite>>(config.CatIconAtlas, timeline, Constants.CacheContext.AGE);
            }
        }
    }
}