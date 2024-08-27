using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Data;
using _Game.Core.Data.Age.Static;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.AgeDataProvider
{
    public class AgeDataProvider : IAgeDataProvider
    {
        private readonly IAgeConfigRepository _ageConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IDataProviderFacade _dataProvider;
        private readonly IMyLogger _logger;
        private readonly IAssetRegistry _assetRegistry;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;

        public AgeDataProvider(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IDataProviderFacade dataProvider,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _ageConfigRepository = configRepositoryFacade.AgeConfigRepository;
            _userContainer = userContainer;
            _dataProvider = dataProvider;
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<AgeStaticData> Load(int timelineId)
        {
            AgeConfig config = _ageConfigRepository.GetAgeConfig(TimelineState.AgeId);
            
            var unitDataPool = LoadUnits(config.Warriors, timelineId);
            var weaponDataPool = LoadWeapons(config.Warriors, timelineId);
            
            var builderTask = LoadUnitBuilderData(config.Warriors, timelineId);
            var baseTask = LoadBase(config, timelineId);
            var unitUpgradeItemTask = LoadUnitUpgradeItems(config.Warriors, timelineId);
            var foodIconTask = LoadFoodIcons(timelineId);
            var towerIconTask = LoadBaseIcon(timelineId);
    
            var results 
                = await UniTask.WhenAll(builderTask, baseTask, unitUpgradeItemTask, foodIconTask, towerIconTask);

            AgeStaticData ageStaticData = new AgeStaticData()
            {
                UnitDataPool = unitDataPool,
                WeaponDataPool = weaponDataPool,
                UnitBuilderDataPool = results.Item1,
                BaseStaticData = results.Item2,
                UnitUpgradesPool = results.Item3,
                FoodIcons = results.Item4,
                TowerHealthIcon = results.Item5,
            };
            
            var previousTimeline = TimelineState.TimelineId - 1;
            _assetRegistry.ClearContext(previousTimeline, Constants.CacheContext.AGE);
            return ageStaticData;
        }

    private async UniTask<Sprite> LoadBaseIcon(int timelineId)
        {
            _logger.Log("Base icon loading");
            return await _dataProvider.LoadBaseIcon(
                new LoadContext(){CacheContext = Constants.CacheContext.BATTLE, Timeline = timelineId});
        }

        private async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(int timelineId)
        {
            _logger.Log("Food icon loading");
            return await _dataProvider.LoadFoodIcons(
                new LoadContext(){CacheContext = Constants.CacheContext.BATTLE, Timeline = timelineId});
        }

        private async UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(IEnumerable<WarriorConfig> configs, int timelineId)
        {
            _logger.Log("Upgrade items loading");
            return await _dataProvider
                .LoadUnitUpgradeItems(configs,
                    new LoadContext()
                    {
                        Faction = Faction.Player,
                        Race = RaceState.CurrentRace,
                        Timeline = timelineId,
                        CacheContext = Constants.CacheContext.AGE
                    });
        }

        private async UniTask<BaseStaticData> LoadBase(AgeConfig config, int timelineId)
        {
            var baseLoadOptions = new BaseLoadOptions()
            {
                Faction = Faction.Player,
                CacheContext = Constants.CacheContext.AGE,
                Timeline = timelineId,
                CoinsAmount = 0,
                BasePrefab = config.BasePrefab
            };
            
            _logger.Log("Bases loading");
            
            return await _dataProvider.LoadBase(baseLoadOptions);
        }

        private async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(
            IEnumerable<WarriorConfig> configs,
            int timelineId)
        {
            _logger.Log("Units builder data loading");
            
            return await _dataProvider
                .LoadUnitBuilderData(
                    configs,
                    new LoadContext()
                    {
                        Faction = Faction.Player,
                        Race = RaceState.CurrentRace,
                        Timeline = timelineId,
                        CacheContext = Constants.CacheContext.AGE
                    });
        }

        private DataPool<int, WeaponData> LoadWeapons(
            IEnumerable<WarriorConfig> configs,
            int timelineId)
        {
            _logger.Log("Weapons loading");
            return _dataProvider.LoadWeapons(configs,
                new LoadContext() {Faction = Faction.Player, Timeline = timelineId, CacheContext = Constants.CacheContext.AGE});
        }

        private DataPool<UnitType, UnitData> LoadUnits(
            IEnumerable<WarriorConfig> configs,
            int timelineId)
        {
            _logger.Log("Units loading");
            return _dataProvider.LoadUnits(configs,
                new LoadContext() {Faction = Faction.Player, Timeline = timelineId, CacheContext = Constants.CacheContext.AGE});
        }
    }
}