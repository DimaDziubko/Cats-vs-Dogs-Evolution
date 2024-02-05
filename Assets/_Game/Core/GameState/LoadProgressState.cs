using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Providers;
using _Game.Core.Loading;
using _Game.Core.Login;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using _Game.Gameplay.GamePlayManager;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
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
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IShopPopupProvider _shopPopupProvider;
        private readonly IAudioService _audioService;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly IPauseManager _pauseManager;
        private readonly IAlertPopupProvider _alertPopupProvider;
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly IBeginGameManager _beginGameManager;
        private readonly IGameConfigController _gameConfigController;
        private readonly IBattleStateService _battleState;
        private readonly IHeader _header;

        public LoadProgressState(
            GameStateMachine stateMachine,
            IPersistentDataService persistentData,
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IAddressableAssetProvider addressableAssetProvider,
            IUserStateCommunicator communicator,
            IAssetProvider assetProvider,
            IRandomService random,
            ISettingsPopupProvider settingsPopupProvider,
            IShopPopupProvider shopPopupProvider,
            IAudioService audioService,

            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            
            ILoadingScreenProvider loadingProvider,
            
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,

            IMainMenuProvider mainMenuProvider,
            IBeginGameManager beginGameManager,
            
            IGameConfigController gameConfigController,

            IBattleStateService battleState,
            
            IHeader header
            )
        {
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;

            _mainMenuProvider = mainMenuProvider;
            _beginGameManager = beginGameManager;
            
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _addressableAssetProvider = addressableAssetProvider;
            _communicator = communicator;
            _assetProvider = assetProvider;
            _random = random;
            _settingsPopupProvider = settingsPopupProvider;
            _shopPopupProvider = shopPopupProvider;
            _audioService = audioService;

            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;

            _loadingProvider = loadingProvider;

            _gameConfigController = gameConfigController;

            _battleState = battleState;

            _header = header;
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
            
            loadingOperations.Enqueue(new AssetLoadingOperation(
                _assetProvider,
                _gameConfigController));

            loadingOperations.Enqueue(
                new GameLoadingOperation(
                    _sceneLoader,
                    _cameraService,
                    _random,
                    _stateMachine,
                    _persistentData,
                    _pauseManager,
                    _alertPopupProvider,
                    _settingsPopupProvider,
                    _shopPopupProvider,
                    _audioService,
                    _communicator,
                    
                    _mainMenuProvider,
                    
                    _beginGameManager,
                    
                    _gameConfigController,
                    _battleState,
                    
                    _header
                ));
            
            _loadingProvider.LoadAndDestroy(loadingOperations).Forget();
        }

        public void Exit()
        {

        }
        
    }
}