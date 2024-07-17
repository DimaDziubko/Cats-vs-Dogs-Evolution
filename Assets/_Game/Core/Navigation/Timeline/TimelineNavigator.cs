﻿using System;
using System.Collections.Generic;
 using _Game.Core.AssetManagement;
 using _Game.Core.Configs.Providers;
 using _Game.Core.DataProviders.AgeDataProvider;
 using _Game.Core.DataProviders.Timeline;
 using _Game.Core.Loading;
 using _Game.Core.Services.UserContainer;
 using Assets._Game.Core._GameInitializer;
 using Assets._Game.Core._Logger;
 using Assets._Game.Core.Data;
 using Assets._Game.Core.DataProviders.BattleDataProvider;
 using Assets._Game.Core.Loading;
 using Assets._Game.Core.LoadingScreen;
 using Assets._Game.Core.UserState;

 namespace _Game.Core.Navigation.Timeline
{
    public class TimelineNavigator : ITimelineNavigator, IDisposable
    {
        public event Action TimelineChanged;

        private readonly IUserContainer _userContainer;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IGameInitializer _gameInitializer;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly IMyLogger _logger;
        private readonly IBattleDataProvider _battleDataProvider;
        private readonly ITimelineDataProvider _timelineDataProvider;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly IAssetRegistry _assetRegistry;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public TimelineNavigator(
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IGameInitializer gameInitializer,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IBattleDataProvider battleDataProvider,
            ITimelineDataProvider timelineDataProvider,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _loadingScreenProvider = loadingScreenProvider;
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _remoteConfigProvider = remoteConfigProvider;
            _battleDataProvider = battleDataProvider;
            _timelineDataProvider = timelineDataProvider;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
            _assetRegistry = assetRegistry;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init() => 
            TimelineState.NextTimelineOpened += MoveToNextTimeline;

        void IDisposable.Dispose()
        {
            TimelineState.NextTimelineOpened += MoveToNextTimeline;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void MoveToNextTimeline()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            
            loadingOperations.Enqueue(new ConfigOperation(
                _userContainer, 
                _remoteConfigProvider,
                _localConfigProvider,
                _logger,
                ConfigurationLevel.Timeline));
            
            loadingOperations.Enqueue(new DataLoadingOperation(
                _generalDataPool,
                _ageDataProvider, 
                _battleDataProvider, 
                _timelineDataProvider,
                _assetRegistry,
                _userContainer,
                _logger));

            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingScreenProvider.LoadAndDestroy(loadingOperations, LoadingScreenType.Transparent);
            
        }

        private void OnLoadingCompleted()
        {
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
            TimelineChanged?.Invoke();
        }
        
    }
}