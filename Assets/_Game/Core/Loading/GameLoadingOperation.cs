using System;
using _Game.Core.Communication;
using _Game.Core.GameState;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.GameModes.BattleMode.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
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
        private readonly IBeginGameManager _beginGameManager;
        private readonly IBattleStateService _battleState;
        private readonly IHeader _header;
        
        private readonly IUpgradesService _upgradesService;
        private readonly IAgeStateService _ageState;

        public string Description => "Game loading...";
        
        public GameLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IRandomService randomService,
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,

            IAudioService audioService,
            IUserStateCommunicator communicator,

            IBeginGameManager beginGameManager,

            IBattleStateService battleState,
            
            IHeader header,
            IUpgradesService upgradesService,
            IAgeStateService ageState)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _randomService = randomService;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;

            _audioService = audioService;
            _communicator = communicator;

            _beginGameManager = beginGameManager;

            _battleState = battleState;

            _header = header;

            _ageState = ageState;
            _upgradesService = upgradesService;
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

            await _ageState.Init();
            await _upgradesService.Init();
            await _battleState.Init();
            
            onProgress?.Invoke(0.85f);
            gameMode.Construct(
                _cameraService,
                _randomService,    
                _stateMachine,
                _persistentData,
                _pauseManager,
                _alertPopupProvider,
                _audioService,
                _communicator,
                _beginGameManager,
                _battleState,
                _header,
                _ageState,
                _upgradesService);

            onProgress?.Invoke(1.0f);
            
            _stateMachine.Enter<MenuState>();
        }
    }
}