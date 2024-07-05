using System.Collections.Generic;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Core.Data.Age.Static;
using Assets._Game.Core.DataProviders.BaseDataProvider;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.AgeDataProvider
{
    public class AgeDataProvider : IAgeDataProvider
    {
        private readonly IAgeConfigRepository _ageConfigRepository;
        private readonly IUserContainer _persistentData;
        private readonly IDataProviderFacade _dataProvider;
        private readonly ICommonItemsConfigRepository _commonItemsConfigRepository;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;

        public AgeDataProvider(
            IAgeConfigRepository ageConfigRepository,
            ICommonItemsConfigRepository commonItemsConfigRepository,
            IUserContainer persistentData,
            IDataProviderFacade dataProvider,
            IMyLogger logger)
        {
            _logger = logger;
            _ageConfigRepository = ageConfigRepository;
            _persistentData = persistentData;
            _dataProvider = dataProvider;
            _commonItemsConfigRepository = commonItemsConfigRepository;
        }
        
        public async UniTask<AgeStaticData> Load()
        {
            AgeConfig config = _ageConfigRepository.GetAgeConfig(TimelineState.AgeId);
            
            var unitTask = LoadUnits(config.Warriors);
            var weaponTask = LoadWeapons(config.Warriors);
            var builderTask = LoadUnitBuilderData(config.Warriors);
            var baseTask = LoadBase(config);
            var unitUpgradeItemTask = LoadUnitUpgradeItems(config.Warriors);
            var foodIconTask = LoadFoodIcons(_commonItemsConfigRepository);
            var towerIconTask = LoadTowerIcon(_commonItemsConfigRepository);
    
            var results = await UniTask.WhenAll(
                unitTask, weaponTask, builderTask, baseTask, unitUpgradeItemTask, foodIconTask, towerIconTask);

            AgeStaticData ageStaticData = new AgeStaticData()
            {
                UnitDataPool = results.Item1,
                WeaponDataPool = results.Item2,
                UnitBuilderDataPool = results.Item3,
                BaseStaticData = results.Item4,
                UnitUpgradesPool = results.Item5,
                FoodIcons = results.Item6,
                TowerHealthIcon = results.Item7,
            };

            return ageStaticData;
        }

        private async UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository) => 
            await _dataProvider.LoadTowerIcon(itemsConfigRepository, Constants.CacheContext.AGE);

        private async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository) => 
            await _dataProvider.LoadFoodIcons(itemsConfigRepository, Constants.CacheContext.AGE);

        private async UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(IEnumerable<WarriorConfig> configs)
        {
            return await _dataProvider
                .LoadUnitUpgradeItems(configs,
                    new LoadContext()
                    {
                        Faction = Faction.Player,
                        Race = RaceState.CurrentRace,
                        CacheContext = Constants.CacheContext.AGE
                    });
        }

        private async UniTask<BaseStaticData> LoadBase(AgeConfig config)
        {
            string key = config.BaseKey;
            
            var baseLoadOptions = new BaseLoadOptions()
            {
                Faction = Faction.Player,
                CacheContext = Constants.CacheContext.AGE,
                PrefabKey = key,
                CoinsAmount = 0,
            };
            
            return await _dataProvider.LoadBase(baseLoadOptions);
        }

        private async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(IEnumerable<WarriorConfig> configs) =>
            await _dataProvider
                .LoadUnitBuilderData(
                    configs, 
                    new LoadContext()
                    {
                        Faction = Faction.Player,
                        Race = RaceState.CurrentRace,
                        CacheContext = Constants.CacheContext.AGE
                    });

        private async UniTask<DataPool<WeaponType, WeaponData>> LoadWeapons(IEnumerable<WarriorConfig> configs) => 
            await _dataProvider.LoadWeapons(configs,  new LoadContext() { Faction = Faction.Player, CacheContext = Constants.CacheContext.AGE});

        private async UniTask<DataPool<UnitType, UnitData>> LoadUnits(IEnumerable<WarriorConfig> configs) => 
            await  _dataProvider.LoadUnits(configs,  new LoadContext() { Faction = Faction.Player, CacheContext = Constants.CacheContext.AGE});
    }
}