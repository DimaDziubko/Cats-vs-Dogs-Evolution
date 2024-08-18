using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Assets._Game.Core.Data;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
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
            string iconKey = options.Config.GetUnitIconKeyForRace(options.CurrentRace);
            var unitIcon = await _assetRegistry.LoadAsset<Sprite>(
                iconKey, options.Timeline, Constants.CacheContext.AGE);
            
            _logger.Log($"Unit builder data with id {options.Config.Id} load successfully");
            
            return new UnitBuilderBtnStaticData
            {
                Type = options.Config.Type,
                UnitIcon = unitIcon,
                FoodPrice = options.Config.FoodPrice,
            };
        }
    }
}