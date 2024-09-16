using System;
using System.Collections.Generic;
 using _Game.Core._DataLoaders.AgeDataProvider;
 using _Game.Core._GameInitializer;
 using _Game.Core._Logger;
 using _Game.Core.AssetManagement;
 using _Game.Core.Configs.Providers;
 using _Game.Core.Data;
 using _Game.Core.DataProviders.BattleDataProvider;
 using _Game.Core.DataProviders.ShopDataProvider;
 using _Game.Core.DataProviders.Timeline;
 using _Game.Core.Loading;
 using _Game.Core.LoadingScreen;
 using _Game.Core.Services.UserContainer;
 using Assets._Game.Core.Loading;
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
        private readonly IAgeDataLoader _ageDataLoader;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly IMyLogger _logger;
        private readonly IBattleDataLoader _battleDataLoader;
        private readonly ITimelineDataLoader _timelineDataLoader;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IShopDataLoader _shopDataLoader;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public TimelineNavigator(
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IGeneralDataPool generalDataPool,
            IAgeDataLoader ageDataLoader,
            IGameInitializer gameInitializer,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IBattleDataLoader battleDataLoader,
            ITimelineDataLoader timelineDataLoader,
            IShopDataLoader shopDataLoader,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _loadingScreenProvider = loadingScreenProvider;
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            _generalDataPool = generalDataPool;
            _ageDataLoader = ageDataLoader;
            _remoteConfigProvider = remoteConfigProvider;
            _battleDataLoader = battleDataLoader;
            _timelineDataLoader = timelineDataLoader;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
            _assetRegistry = assetRegistry;
            _shopDataLoader = shopDataLoader;
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
                _ageDataLoader, 
                _battleDataLoader, 
                _timelineDataLoader,
                _shopDataLoader,
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