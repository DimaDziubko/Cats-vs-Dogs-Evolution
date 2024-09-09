using System.Collections.Generic;
using _Game.Core._DataLoaders.Facade;
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
using _Game.Utils;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.AgeDataProvider
{
    public class AgeDataLoader : IAgeDataLoader
    {
        private readonly IAgeConfigRepository _ageConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IDataLoaderFacade _dataLoader;
        private readonly IMyLogger _logger;
        private readonly IAssetRegistry _assetRegistry;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;

        public AgeDataLoader(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IDataLoaderFacade dataLoader,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _ageConfigRepository = configRepositoryFacade.AgeConfigRepository;
            _userContainer = userContainer;
            _dataLoader = dataLoader;
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<AgeStaticData> Load(int timelineId)
        {
            AgeConfig config = _ageConfigRepository.GetAgeConfig(TimelineState.AgeId);

            var unitTask = LoadUnitsAsync(config.Warriors, timelineId);
            var weaponDataPool = LoadWeapons(config.Warriors, timelineId);
            var baseTask = LoadBase(config, timelineId);

            var results 
                = await UniTask.WhenAll(unitTask, baseTask);

            AgeStaticData ageStaticData = new AgeStaticData()
            {
                UnitDataPool = results.Item1,
                WeaponDataPool = weaponDataPool,
                BaseStaticData = results.Item2,
            };
            
            var previousTimeline = TimelineState.TimelineId - 1;
            _assetRegistry.ClearContext(previousTimeline, Constants.CacheContext.AGE);
            return ageStaticData;
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
            
            return await _dataLoader.LoadBase(baseLoadOptions);
        }

        private DataPool<int, WeaponData> LoadWeapons(
            IEnumerable<WarriorConfig> configs,
            int timelineId)
        {
            _logger.Log("Weapons loading");
            return _dataLoader.LoadWeapons(configs,
                new LoadContext() {Faction = Faction.Player, Timeline = timelineId, CacheContext = Constants.CacheContext.AGE});
        }

        private async UniTask<DataPool<UnitType, IUnitData>> LoadUnitsAsync(
            IEnumerable<WarriorConfig> configs,
            int timelineId)
        {
            _logger.Log("Units loading");
            return await _dataLoader.LoadUnitsAsync(configs,
                new LoadContext() {Faction = Faction.Player, Timeline = timelineId, CacheContext = Constants.CacheContext.AGE});
        }
    }
}