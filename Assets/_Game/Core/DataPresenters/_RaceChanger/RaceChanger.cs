using System;
using _Game.Core._GameInitializer;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.DataProviders.AgeDataProvider;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.LoadingScreen;
using Assets._Game.Core.UserState;

namespace _Game.Core.DataPresenters._RaceChanger
{
    public class RaceChanger : IRaceChanger, IDisposable
    {
        public event Action RaceChanged;
        
        private readonly IUserContainer _useContainer;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IBattleDataProvider _battleDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IGameInitializer _gameInitializer;
        private IRaceStateReadonly RaceState => _useContainer.State.RaceState;

        public RaceChanger(
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IUserContainer useContainer,
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IBattleDataProvider battleDataProvider,
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
            _battleDataProvider = battleDataProvider;
            gameInitializer.OnPostInitialization += Init;
        }
        
        private void Init() =>  
            RaceState.Changed += OnRaceChanged;

        void IDisposable.Dispose()
        {
            RaceState.Changed -= OnRaceChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnRaceChanged()
        {
            var raceChangingOperation = new ChangingRaceOperation(
                _generalDataPool,
                _ageDataProvider,
                _battleDataProvider,
                _assetRegistry,
                _useContainer);
            
            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingScreenProvider.LoadAndDestroy(raceChangingOperation, LoadingScreenType.Simple);
        }

        private void OnLoadingCompleted()
        {
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
            RaceChanged?.Invoke();
        }
    }
}