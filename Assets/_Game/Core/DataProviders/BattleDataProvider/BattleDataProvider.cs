﻿using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.Data.Battle;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._Environment;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.BattleDataProvider
{
    public class BattleDataProvider : IBattleDataProvider
    {
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IUserContainer _persistentData;
        private readonly IMyLogger _logger;
        private readonly IDataProviderFacade _dataProvider;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public BattleDataProvider(
            ITimelineConfigRepository timelineConfigRepository,
            IUserContainer persistentData,
            IDataProviderFacade dataProvider,
            IMyLogger logger)
        {
            _logger = logger;
            _timelineConfigRepository = timelineConfigRepository;
            _persistentData = persistentData;
            _dataProvider = dataProvider;
        }
        
        public async UniTask<BattleStaticData> Load()
        {
            IEnumerable<BattleConfig> battleConfigs = _timelineConfigRepository.GetBattleConfigs();
            
            var unitTask = LoadUnits(battleConfigs);
            var weaponTask = LoadWeapons(battleConfigs);
            var baseTask = LoadBases(battleConfigs);
            var environmentTask = LoadEnvironments(battleConfigs);
            var ambienceTask = LoadAmbience(battleConfigs);
            var battleData = LoadBattleScenarioData(battleConfigs);
            var towerHealth = LoadTowerHealth(battleConfigs);
            
            var result = await UniTask.WhenAll(unitTask, weaponTask, baseTask, environmentTask, ambienceTask);
            
            BattleStaticData ageStaticData = new BattleStaticData()
            {
                UnitDataPools = result.Item1,
                WeaponDataPools = result.Item2,
                BasePool = result.Item3,
                EnvironmentPool = result.Item4,
                BattleDataPools = battleData,
                AmbiencePool = result.Item5,
                BaseHealthPool = towerHealth
            };

            return ageStaticData;
        }

        private Dictionary<int, float> LoadTowerHealth(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, float> data = new Dictionary<int, float>();
            int battleIndex = 0;
            foreach (var config in configs)
            {
                data.Add(battleIndex,  config.EnemyTowerHealth);
                battleIndex++;
            }
            
            return data;
        }

        private async UniTask<Dictionary<int, AudioClip>> LoadAmbience(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, AudioClip> data = new Dictionary<int, AudioClip>();
            int battleIndex = 0;
            foreach (var config in configs)
            {
                var ambience =  await _dataProvider.LoadAmbience(config.AmbienceKey, Constants.CacheContext.BATTLE);
                data.Add(battleIndex, ambience);
                battleIndex++;
            }

            return data;
        }

        private Dictionary<int, BattleScenarioData> LoadBattleScenarioData(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, BattleScenarioData> pool = new Dictionary<int, BattleScenarioData>();

            int battleIndex = 0;
            
            foreach (var config in configs)
            {
                BattleScenarioData scenarioData = new BattleScenarioData
                {
                    Scenario = config.Scenario,
                    MaxCoinsPerBattle = config.MaxCoinsPerBattle,
                    AnalyticsData = new BattleAnalyticsData()
                    {
                        TimelineNumber = TimelineState.TimelineId + 1,
                        AgeNumber = TimelineState.AgeId + 1,
                        BattleNumber = battleIndex + 1
                    }
                };
                
                pool.Add(battleIndex, scenarioData);
                battleIndex++;
            }

            return pool;
        }

        private async UniTask<Dictionary<int, EnvironmentData>> LoadEnvironments(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, EnvironmentData> environmentPool = new Dictionary<int, EnvironmentData>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                var data = await _dataProvider.LoadEnvironment(config.EnvironmentKey, Constants.CacheContext.BATTLE);
                environmentPool.Add(battleIndex, data);
                battleIndex++;
            }

            return environmentPool;
        }

        private async UniTask<Dictionary<int, BaseStaticData>> LoadBases(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, BaseStaticData> basePool = new Dictionary<int, BaseStaticData>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                string key = config.BaseKey;
                var baseLoadOptions = new BaseLoadOptions()
                {
                    Faction = Faction.Enemy,
                    CacheContext = Constants.CacheContext.BATTLE,
                    PrefabKey = key,
                    CoinsAmount = config.CoinsPerBase,
                };

                BaseStaticData staticData = await _dataProvider.LoadBase(baseLoadOptions);
                
                basePool.Add(battleIndex, staticData);
                battleIndex++;
            }

            return basePool;
        }

        private async UniTask<Dictionary<int, DataPool<WeaponType, WeaponData>>> LoadWeapons(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, DataPool<WeaponType, WeaponData>> weaponDataPools = new Dictionary<int, DataPool<WeaponType, WeaponData>>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                DataPool<WeaponType, WeaponData> dataPool = 
                    await _dataProvider
                        .LoadWeapons(
                            config.Enemies, 
                            new LoadContext()
                            {
                                Faction = Faction.Enemy,
                                CacheContext = Constants.CacheContext.BATTLE
                            });
                
                weaponDataPools.Add(battleIndex, dataPool);
                battleIndex++;
            }

            return weaponDataPools;
        }

        private async UniTask<Dictionary<int, DataPool<UnitType, UnitData>>> LoadUnits(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, DataPool<UnitType, UnitData>> unitDataPools = new Dictionary<int, DataPool<UnitType, UnitData>>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                DataPool<UnitType, UnitData> dataPool = 
                    await _dataProvider
                        .LoadUnits(
                            config.Enemies,
                             new LoadContext()
                            {
                                Faction = Faction.Enemy,
                                CacheContext = Constants.CacheContext.BATTLE
                            });
                
                unitDataPools.Add(battleIndex, dataPool);
                battleIndex++;
            }

            return unitDataPools;
        }
    }
}