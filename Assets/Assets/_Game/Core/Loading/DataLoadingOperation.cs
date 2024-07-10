using System;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Core.Data.Age.Dynamic;
using Assets._Game.Core.DataProviders.AgeDataProvider;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.DataProviders.Timeline;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.Loading
{
    public class DataLoadingOperation : ILoadingOperation
    {
        public string Description => "Loading resources...";
        
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IBattleDataProvider _battleDataProvider;
        private readonly ITimelineDataProvider _timelineDataProvider;
        private readonly IMyLogger _logger;


        public DataLoadingOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IBattleDataProvider baseDataProvider,
            ITimelineDataProvider timelineDataProvider,
            IMyLogger logger)
        {
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _battleDataProvider = baseDataProvider;
            _timelineDataProvider = timelineDataProvider;
            _logger = logger;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.3f);
            UniTask timelineTask = LoadTimelineData();
            UniTask ageTask = LoadAgeData();
            UniTask battleTask = LoadBattleData();
            await UniTask.WhenAll(ageTask, battleTask, timelineTask);
            onProgress.Invoke(1);
        }

        private async UniTask LoadTimelineData() => 
            _generalDataPool.TimelineStaticData = await _timelineDataProvider.Load();

        private async UniTask LoadBattleData() => 
            _generalDataPool.BattleStaticData = await _battleDataProvider.Load();

        private async UniTask LoadAgeData()
        {
            _generalDataPool.AgeStaticData = await _ageDataProvider.Load();
            _generalDataPool.AgeDynamicData = new AgeDynamicData();
        }
    }
}