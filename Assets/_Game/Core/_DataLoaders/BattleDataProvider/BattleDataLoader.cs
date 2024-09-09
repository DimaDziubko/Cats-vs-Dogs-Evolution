using System.Collections.Generic;
using _Game.Core._DataLoaders.Facade;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.Data.Battle;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using _Game.Utils;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.BattleDataProvider
{
    public class BattleDataLoader : IBattleDataLoader
    {
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IDataLoaderFacade _dataLoader;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public BattleDataLoader(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IDataLoaderFacade dataLoader,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
            _userContainer = userContainer;
            _dataLoader = dataLoader;
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<BattleStaticData> Load(int timelineId)
        {
            IEnumerable<BattleConfig> battleConfigs = _timelineConfigRepository.GetBattleConfigs();

            var weaponDataPool = LoadWeapons(battleConfigs, timelineId);

            var unitTask = LoadUnitsAsync(battleConfigs, timelineId);
            var baseTask = LoadBases(battleConfigs, timelineId);
            var environmentTask = LoadEnvironments(battleConfigs, timelineId);
            var ambienceTask = LoadAmbience(battleConfigs, timelineId);
            var battleData = LoadBattleScenarioData(battleConfigs);
            var baseHealth = LoadBaseHealth(battleConfigs);

            var result = await UniTask.WhenAll(unitTask, baseTask, environmentTask, ambienceTask);
            
            BattleStaticData ageStaticData = new BattleStaticData()
            {
                UnitDataPools = result.Item1,
                WeaponDataPools = weaponDataPool,
                BasePool = result.Item2,
                EnvironmentPool = result.Item3,
                BattleDataPools = battleData,
                AmbiencePool = result.Item4,
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
                var ambience =  await _dataLoader.LoadAmbience(config.AmbienceKey,
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
                var data = await _dataLoader.LoadEnvironment(config.EnvironmentKey, 
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
                var baseLoadOptions = new BaseLoadOptions()
                {
                    Faction = Faction.Enemy,
                    CacheContext = Constants.CacheContext.BATTLE,
                    Timeline = timelineId,
                    CoinsAmount = config.CoinsPerBase,
                    BasePrefab = config.BasePrefab
                };

                BaseStaticData staticData = await _dataLoader.LoadBase(baseLoadOptions);
                
                basePool.Add(battleIndex, staticData);
                battleIndex++;
            }

            return basePool;
        }

        private Dictionary<int, DataPool<int, WeaponData>> LoadWeapons(
            IEnumerable<BattleConfig> configs,
            int timelineId)
        {
            Dictionary<int, DataPool<int, WeaponData>> weaponDataPools = new Dictionary<int, DataPool<int, WeaponData>>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                DataPool<int, WeaponData> dataPool = 
                     _dataLoader
                        .LoadWeapons(
                            config.Warriors, 
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

        private async UniTask<Dictionary<int, DataPool<UnitType, IUnitData>>> LoadUnitsAsync(IEnumerable<BattleConfig> configs, int timelineId)
        {
            Dictionary<int, DataPool<UnitType, IUnitData>> unitDataPools = new Dictionary<int, DataPool<UnitType, IUnitData>>();
            
            int battleIndex = 0;
            foreach (var config in configs)
            {
                DataPool<UnitType, IUnitData> dataPool = await 
                    _dataLoader
                        .LoadUnitsAsync(
                            config.Warriors,
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