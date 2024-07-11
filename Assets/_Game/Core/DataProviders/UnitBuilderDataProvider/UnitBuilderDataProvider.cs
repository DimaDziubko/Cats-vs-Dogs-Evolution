using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.UnitBuilderDataProvider
{
    public class UnitBuilderDataProvider : IUnitBuilderDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _persistentData;

        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        
        public UnitBuilderDataProvider(
            IAssetRegistry assetRegistry,
            IUserContainer persistentData,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _persistentData = persistentData;
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
                dataPool.Add(config.Type, staticData);
            }

            return dataPool;
        }

        private async UniTask<UnitBuilderBtnStaticData> LoadData(BuilderLoadOptions options)
        {
            string iconKey = options.Config.GetUnitIconKeyForRace(options.CurrentRace);
            var unitIcon = await _assetRegistry.LoadAsset<Sprite>(iconKey, Constants.CacheContext.AGE);
            
            return new UnitBuilderBtnStaticData
            {
                Type = options.Config.Type,
                UnitIcon = unitIcon,
                FoodPrice = options.Config.FoodPrice,
            };
        }
    }
}