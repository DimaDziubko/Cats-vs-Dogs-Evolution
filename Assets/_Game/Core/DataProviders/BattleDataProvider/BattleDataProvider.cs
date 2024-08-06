using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.Data.Battle;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using _Game.Utils;
using Assets._Game.Core._Logger;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.BattleDataProvider
{
    public class BattleDataProvider : IBattleDataProvider
    {
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IDataProviderFacade _dataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public BattleDataProvider(
            ITimelineConfigRepository timelineConfigRepository,
            IUserContainer userContainer,
            IDataProviderFacade dataProvider,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _timelineConfigRepository = timelineConfigRepository;
            _userContainer = userContainer;
            _dataProvider = dataProvider;
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<BattleStaticData> Load(int timelineId)
        {
            IEnumerable<BattleConfig> battleConfigs = _timelineConfigRepository.GetBattleConfigs();
            
            var unitTask = LoadUnits(battleConfigs, timelineId);
            var baseTask = LoadBases(battleConfigs, timelineId);
            var environmentTask = LoadEnvironments(battleConfigs, timelineId);
            var ambienceTask = LoadAmbience(battleConfigs, timelineId);
            var battleData = LoadBattleScenarioData(battleConfigs);
            var baseHealth = LoadBaseHealth(battleConfigs);
            var weaponTask = LoadWeapons(battleConfigs, timelineId);

            var result = await UniTask.WhenAll(unitTask, weaponTask, baseTask, environmentTask, ambienceTask);
            
            BattleStaticData ageStaticData = new BattleStaticData()
            {
                UnitDataPools = result.Item1,
                WeaponDataPools = result.Item2,
                BasePool = result.Item3,
                EnvironmentPool = result.Item4,
                BattleDataPools = battleData,
                AmbiencePool = result.Item5,
                BaseHealthPool = baseHealth
            };

            var previousTimeline = TimelineState.TimelineId - 1;
            _assetRegistry.ClearContext(previousTimeline , Constants.CacheContext.BATTLE);
            return ageStaticData;
        }

        private Dictionary<int, float> LoadBaseHealth(IEnumerable<BattleConfig> configs)
        {
            Dictionary<int, float> data = new Dictionary<int, float>();
            int battleIndex = 0;
            foreach (var config in configs)
            {
                data.Add(battleIndex,  config.EnemyBaseHealth);
                battleIndex++;
            }
            
            return data;
        }

        private async UniTask<Dictionary<int, AudioClip>> LoadAmbience(IEnumerable<BattleConfig> configs, int timelineId)
        {
            Dictionary<int, AudioClip> data = new Dictionary<int, AudioClip>();
            int battleIndex = 0;
            foreach (var config in configs)
            {
                var ambience =  await _dataProvider.LoadAmbience(config.AmbienceKey,
                    new LoadContext(){CacheContext = Constants.CacheContext.BATTLE, Timeline = timelineId});
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

        private async UniTask<Dictionary<int, EnvironmentData>> LoadEnvironments(IEnumerable<BattleConfig> configs, int timelineId)
        {
            Dictionary<int, EnvironmentData> environmentPool = new Dictionary<int, EnvironmentData>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                var data = await _dataProvider.LoadEnvironment(config.EnvironmentKey, 
                    new LoadContext(){CacheContext = Constants.CacheContext.BATTLE, Timeline = timelineId});
                environmentPool.Add(battleIndex, data);
                battleIndex++;
            }

            return environmentPool;
        }

        private async UniTask<Dictionary<int, BaseStaticData>> LoadBases(IEnumerable<BattleConfig> configs, int timelineId)
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
                    Timeline = timelineId,
                    CoinsAmount = config.CoinsPerBase,
                };

                BaseStaticData staticData = await _dataProvider.LoadBase(baseLoadOptions);
                
                basePool.Add(battleIndex, staticData);
                battleIndex++;
            }

            return basePool;
        }

        private async UniTask<Dictionary<int, DataPool<int, WeaponData>>> LoadWeapons(
            IEnumerable<BattleConfig> configs,
            int timelineId)
        {
            Dictionary<int, DataPool<int, WeaponData>> weaponDataPools = new Dictionary<int, DataPool<int, WeaponData>>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                DataPool<int, WeaponData> dataPool = 
                    await _dataProvider
                        .LoadWeapons(
                            config.Enemies, 
                            new LoadContext()
                            {
                                Faction = Faction.Enemy,
                                Timeline = timelineId,
                                CacheContext = Constants.CacheContext.BATTLE
                            });
                
                weaponDataPools.Add(battleIndex, dataPool);
                battleIndex++;
            }

            return weaponDataPools;
        }

        private async UniTask<Dictionary<int, DataPool<UnitType, UnitData>>> LoadUnits(IEnumerable<BattleConfig> configs, int timelineId)
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
                                Timeline = timelineId,
                                CacheContext = Constants.CacheContext.BATTLE
                            });
                
                unitDataPools.Add(battleIndex, dataPool);
                battleIndex++;
            }

            return unitDataPools;
        }
    }
}