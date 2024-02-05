using System.Collections.Generic;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Factory;
using _Game.Core.GameState;
using _Game.Core.Pause.Scripts;
using _Game.Core.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.UserState;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Unit.Factory;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.GameResult.Scripts;
using _Game.UI.Hud;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.Utils;
using _Game.Utils.Popups;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.GameModes.BattleMode.Scripts
{
    public class BattleMode : MonoBehaviour, IGameModeCleaner, IBeginGameHandler
    {
        public IEnumerable<GameObjectFactory> Factories { get; private set; }
        public string SceneName => Constants.Scenes.BATTLE_MODE;

        private IWorldCameraService _cameraService;
        private SceneLoader _sceneLoader;
        private IGameStateMachine _stateMachine;
        private IPersistentDataService _persistentData;
        private IPauseManager _pauseManager;
        private IAudioService _audioService;
        private IUserStateCommunicator  _communicator;
        private IAlertPopupProvider _alertPopupProvider;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        
        private IMainMenuProvider _mainMenuProvider;
        
        private IBeginGameManager _beginGameManager;
        
        private IGameConfigController _gameConfigController;

        [SerializeField] private Hud _hud;

        [SerializeField] private GameResultWindow _gameResultWindow;

        //[SerializeField] private BattleScenarioExecutor _scenario;

        //[SerializeField] private BattleField _battleField;

        [SerializeField] private Battle _battle;

        //private BattleScenarioExecutor.State _activeScenario;
        private bool IsPaused => _pauseManager.IsPaused;
        private bool _scenarioInProcess;

        //private readonly GameBehaviourCollection _units = new GameBehaviourCollection();
        
        public void Construct(
            IWorldCameraService cameraService,
            IRandomService random,
            SceneLoader sceneLoader,
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
            _cameraService = cameraService;
            _sceneLoader = sceneLoader;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;

            _mainMenuProvider = mainMenuProvider;

            _beginGameManager = beginGameManager;

            _gameConfigController = gameConfigController;
            
            InitializeCameras(cameraService);
            
            _gameResultWindow.Construct(_cameraService);
            
            _hud.Construct(
                _cameraService,
                _pauseManager,
                _alertPopupProvider,
                _audioService);

            
            _battle.Construct(
                battleState);

            //TODO unregister if necessary
            _beginGameManager.Register(this);
            
            header.ShowWallet(persistentData);
        }

        [Button]
        private void MenuState()
        {
            _stateMachine.Enter<MenuState>();
        }
        
        [Button]
        private void GameLoopState()
        {
            _stateMachine.Enter<GameLoopState>();
        }
        
        [Inject]
        private void InitializeFactories(IUnitFactory unitFactory)
        {
            Factories = new GameObjectFactory[]
            {
               unitFactory as GameObjectFactory, 
            };
        }

        void IBeginGameHandler.BeginGame()
        {
            //TODO Delete 
            Debug.Log("Begin new game");
            
            Cleanup();
            _hud.Show();
            _hud.QuitGame += GoToMainMenu;
            _battle.StartBattle();
            _stateMachine.Enter<GameLoopState>();
        }
        
        public void Cleanup()
        {
            _hud.QuitGame -= GoToMainMenu;
            _battle.Cleanup();
        }
        
        private void InitializeCameras(IWorldCameraService cameraService)
        {
            var uICameraOverlay = cameraService.UICameraOverlay;
            cameraService.AddUICameraToStack(uICameraOverlay);
        }

        private void Update()
        {
            if(IsPaused) return;
         
            if (Input.GetKeyDown(KeyCode.R))
            {
                ((IBeginGameHandler) this).BeginGame();
            }

            _battle.GameUpdate();
        }

        private void SaveGame() => _communicator.SaveUserState(_persistentData.State);

        private int CalculateAward()
        {
            //TODO Implement later
            return 0;
        }

        private void GoToMainMenu()
        {
            _hud.Hide();
            _stateMachine.Enter<MenuState>();
        }
    }
}