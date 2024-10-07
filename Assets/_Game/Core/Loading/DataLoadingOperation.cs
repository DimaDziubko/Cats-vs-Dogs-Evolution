using System;
using _Game.Core._DataLoaders.AgeDataProvider;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic;
using _Game.Core.DataProviders.BattleDataProvider;
using _Game.Core.DataProviders.Timeline;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.Loading;
using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class DataLoadingOperation : ILoadingOperation
    {
        public string Description => "Loading resources...";
        
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataLoader _ageDataLoader;
        private readonly IBattleDataLoader _battleDataLoader;
        private readonly ITimelineDataLoader _timelineDataLoader;
        private readonly IMyLogger _logger;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineStateReadonly => _userContainer.State.TimelineState;
        
        public DataLoadingOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataLoader ageDataLoader,
            IBattleDataLoader baseDataLoader,
            ITimelineDataLoader timelineDataLoader,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _generalDataPool = generalDataPool;
            _ageDataLoader = ageDataLoader;
            _battleDataLoader = baseDataLoader;
            _timelineDataLoader = timelineDataLoader;
            _assetRegistry = assetRegistry; 
            _logger = logger;
            _userContainer = userContainer;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            _assetRegistry.ClearTimeline(TimelineStateReadonly.TimelineId - 1);
            
            await UniTask.Delay(TimeSpan.FromSeconds(1)); //Delay between loading and releasing
            
            onProgress.Invoke(0.3f);
            UniTask timelineTask = LoadTimelineData();
            UniTask ageTask = LoadAgeData();
            UniTask battleTask = LoadBattleData();
            await UniTask.WhenAll(ageTask, battleTask, timelineTask);

            onProgress.Invoke(1);
        }

        private async UniTask LoadTimelineData()
        {
            _generalDataPool.TimelineStaticData = await _timelineDataLoader.Load(TimelineStateReadonly.TimelineId);
            _logger.Log("TimelineData load successfully");
        }

        private async UniTask LoadBattleData()
        {
            _generalDataPool.BattleStaticData = await _battleDataLoader.Load(TimelineStateReadonly.TimelineId);
            _logger.Log("BattleData load successfully");
        }

        private async UniTask LoadAgeData()
        {
            _generalDataPool.AgeStaticData = await _ageDataLoader.Load(TimelineStateReadonly.TimelineId);
            _generalDataPool.AgeDynamicData = new AgeDynamicData();
            _logger.Log("AgeData load successfully");
        }
    }
}