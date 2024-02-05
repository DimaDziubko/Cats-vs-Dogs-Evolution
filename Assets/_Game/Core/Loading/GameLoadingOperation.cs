using System;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.GameState;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.GameModes.BattleMode.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Game.Core.Loading
{
    public sealed class GameLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IRandomService _randomService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IPersistentDataService _persistentData;
        private readonly IPauseManager _pauseManager;
        private readonly IAlertPopupProvider _alertPopupProvider;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IShopPopupProvider _shopPopupProvider;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly IBeginGameManager _beginGameManager;
        private readonly IGameConfigController _gameConfigController;
        private readonly IBattleStateService _battleState;
        private readonly IHeader _header;

        public string Description => "Game loading...";
        
        public GameLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IRandomService randomService,
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            ISettingsPopupProvider settingsPopupProvider,
            IShopPopupProvider shopPopupProvider,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            
            IMainMenuProvider mainMenuProvider,
            
            IBeginGameManager beginGameManager,
            
            IGameConfigController gameConfigController,

            IBattleStateService battleState,
            
            IHeader header)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _randomService = randomService;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _settingsPopupProvider = settingsPopupProvider;
            _shopPopupProvider = shopPopupProvider;
            _audioService = audioService;
            _communicator = communicator;

            _mainMenuProvider = mainMenuProvider;

            _beginGameManager = beginGameManager;

            _gameConfigController = gameConfigController;

            _battleState = battleState;

            _header = header;
        }
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var loadOp = _sceneLoader.LoadSceneAsync(Constants.Scenes.BATTLE_MODE,
                LoadSceneMode.Single);
            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            onProgress?.Invoke(0.7f);
            
            Scene scene = _sceneLoader.GetSceneByName(Constants.Scenes.BATTLE_MODE);
            
            var gameMode = scene.GetRoot<BattleMode>();
            onProgress?.Invoke(0.85f);
            gameMode.Construct(
                _cameraService,
                _randomService,
                _sceneLoader,
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
                
                _header);
            
            //TODO Delete
            //gameMode.BeginGame();
            
            onProgress?.Invoke(1.0f);
            
            _stateMachine.Enter<MenuState>();
        }
    }
}