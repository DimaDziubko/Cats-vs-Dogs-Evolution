using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Configs.Providers;
using _Game.Core.Loading;
using _Game.Core.Login;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.Core.GameState
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IPersistentDataService _persistentData;
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IAddressableAssetProvider _addressableAssetProvider;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAssetProvider _assetProvider;
        private readonly IRandomService _random;

        private readonly IAudioService _audioService;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly IPauseManager _pauseManager;
        private readonly IAlertPopupProvider _alertPopupProvider;
        private readonly IBeginGameManager _beginGameManager;
        private readonly IBattleStateService _battleState;
        private readonly IHeader _header;
        private readonly IUpgradesService _upgradesService;
        private readonly IAgeStateService _ageState;

        public LoadProgressState(
            GameStateMachine stateMachine,
            IPersistentDataService persistentData,
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IAddressableAssetProvider addressableAssetProvider,
            IUserStateCommunicator communicator,
            IAssetProvider assetProvider,
            IRandomService random,

            IAudioService audioService,

            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            
            ILoadingScreenProvider loadingProvider,
            
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            
            IBeginGameManager beginGameManager,
            
            IBattleStateService battleState,
            
            IHeader header,
            IUpgradesService upgradesService,
            IAgeStateService ageState
            )
        {
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _beginGameManager = beginGameManager;
            
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _addressableAssetProvider = addressableAssetProvider;
            _communicator = communicator;
            _assetProvider = assetProvider;
            _random = random;

            _audioService = audioService;

            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;

            _loadingProvider = loadingProvider;

            _battleState = battleState;

            _header = header;

            _upgradesService = upgradesService;
            _ageState = ageState;
        }

        public void Enter()
        {
            GameLoadState();
        }

        private void GameLoadState()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            
            loadingOperations.Enqueue(_addressableAssetProvider);

            loadingOperations.Enqueue(new ConfigOperation(
                _persistentData,
                _assetProvider,
                _remoteConfigProvider,
                _localConfigProvider));
            
            loadingOperations.Enqueue(new LoginOperation(
                _persistentData,
                _communicator,
                _assetProvider,
                _random));

            loadingOperations.Enqueue(
                new GameLoadingOperation(
                    _sceneLoader,
                    _cameraService,
                    _random,
                    _stateMachine,
                    _persistentData,
                    _pauseManager,
                    _alertPopupProvider,

                    _audioService,
                    _communicator,

                    _beginGameManager,
    
                    _battleState,
                    
                    _header,
                    _upgradesService,
                    _ageState
                    ));
            
            _loadingProvider.LoadAndDestroy(loadingOperations).Forget();
        }

        public void Exit()
        {

        }
        
    }
}