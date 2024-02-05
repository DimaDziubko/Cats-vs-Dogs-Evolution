using System.Collections.Generic;
using _Game.Core.Communication;
using _Game.Core.Loading;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Gameplay.GamePlayManager;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.Core.GameState
{
    public class GameLoadState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly GameStateMachine _stateMachine;
        private readonly IWorldCameraService _cameraService;
        private readonly IRandomService _randomService;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly IPersistentDataService _persistentData;
        private readonly IPauseManager _pauseManager;
        private readonly IAlertPopupProvider _alertPopupProvider;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IShopPopupProvider _shopPopupProvider;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        
        private readonly IStartBattleWindowProvider _startBattleWindowProvider;
        private readonly IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly IBeginGameManager _beginGameManager;

        public GameLoadState(
                GameStateMachine stateMachine, 
                SceneLoader sceneLoader,
                IWorldCameraService cameraService,
                IRandomService randomService,
                ILoadingScreenProvider loadingProvider,
                IPersistentDataService persistentData,
                IPauseManager pauseManager,
                IAlertPopupProvider alertPopupProvider,
                ISettingsPopupProvider settingsPopupProvider,
                IShopPopupProvider shopPopupProvider,
                IAudioService audioService,
                IUserStateCommunicator communicator,
                
                IStartBattleWindowProvider startBattleWindowProvider,
                IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider,
                IMainMenuProvider mainMenuProvider,
                
                IBeginGameManager beginGameManager)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _randomService = randomService;
            _loadingProvider = loadingProvider;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _settingsPopupProvider = settingsPopupProvider;
            _shopPopupProvider = shopPopupProvider;
            _audioService = audioService;
            _communicator = communicator;

            _startBattleWindowProvider = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;
            _mainMenuProvider = mainMenuProvider;

            _beginGameManager = beginGameManager;
        }
        
        public void Enter()
        {
            Load();
        }
        
        public void Exit()
        {

        }
        
        private void Load()
        {

        }
    }
}