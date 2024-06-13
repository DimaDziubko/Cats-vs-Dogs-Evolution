﻿using System;
using _Game.Core._GameInitializer;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.DataProviders.AgeDataProvider;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

namespace _Game.Core.Navigation.Age
{
    public class AgeNavigator : IAgeNavigator, IDisposable
    {
        public event Action AgeChanged;
        
        private readonly IUserContainer _useContainer;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IGameInitializer _gameInitializer;
        private ITimelineStateReadonly TimelineState => _useContainer.State.TimelineState;
        
        public AgeNavigator(
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IUserContainer useContainer,
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IAssetRegistry assetRegistry,
            IGameInitializer gameInitializer)
        {
            _useContainer = userContainer;
            _loadingScreenProvider = loadingScreenProvider;
            _gameInitializer = gameInitializer;
            _useContainer = useContainer;
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _assetRegistry = assetRegistry;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextAgeOpened += MoveToNextAge;
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextAgeOpened -= MoveToNextAge;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void MoveToNextAge()
        {
            var ageLoadingOperation = new AgeDataLoadingOperation(
                _generalDataPool,
                _ageDataProvider, 
                _assetRegistry);

            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingScreenProvider.LoadAndDestroy(ageLoadingOperation);
            
        }

        private void OnLoadingCompleted()
        {
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
            AgeChanged?.Invoke();
        }
    }
}