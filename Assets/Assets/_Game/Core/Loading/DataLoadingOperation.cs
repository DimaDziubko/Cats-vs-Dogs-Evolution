﻿using System;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.AgeDataProvider;
using _Game.Core.DataProviders.Timeline;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Core.Data.Age.Dynamic;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.UserState;
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
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineStateReadonly => _userContainer.State.TimelineState;
        
        public DataLoadingOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IBattleDataProvider baseDataProvider,
            ITimelineDataProvider timelineDataProvider,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _battleDataProvider = baseDataProvider;
            _timelineDataProvider = timelineDataProvider;
            _assetRegistry = assetRegistry; 
            _logger = logger;
            _userContainer = userContainer;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.3f);
            UniTask timelineTask = LoadTimelineData();
            UniTask ageTask = LoadAgeData();
            UniTask battleTask = LoadBattleData();
            await UniTask.WhenAll(ageTask, battleTask, timelineTask);
            _assetRegistry.ClearTimeline(TimelineStateReadonly.TimelineId - 1);
            onProgress.Invoke(1);
        }

        private async UniTask LoadTimelineData()
        {
            _generalDataPool.TimelineStaticData = await _timelineDataProvider.Load(TimelineStateReadonly.TimelineId);
            _logger.Log("TimelineData load successfully");
        }

        private async UniTask LoadBattleData()
        {
            _generalDataPool.BattleStaticData = await _battleDataProvider.Load(TimelineStateReadonly.TimelineId);
            _logger.Log("BattleData load successfully");
        }

        private async UniTask LoadAgeData()
        {
            _generalDataPool.AgeStaticData = await _ageDataProvider.Load(TimelineStateReadonly.TimelineId);
            _generalDataPool.AgeDynamicData = new AgeDynamicData();
            _logger.Log("AgeData load successfully");
        }
    }
}