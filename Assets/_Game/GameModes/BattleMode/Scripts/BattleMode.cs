using System.Collections.Generic;
using _Game.Bundles.Bases.Factory;
using _Game.Bundles.Units.Common.Factory;
using _Game.Core.Communication;
using _Game.Core.Factory;
using _Game.Core.GameState;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.GameResult.Scripts;
using _Game.UI.Hud;
using _Game.Utils;
using _Game.Utils.Popups;
using UnityEngine;
using Zenject;

namespace _Game.GameModes.BattleMode.Scripts
{
    public class BattleMode : MonoBehaviour, IGameModeCleaner, IBeginGameHandler
    {
        public IEnumerable<GameObjectFactory> Factories { get; private set; }
        public string SceneName => Constants.Scenes.BATTLE_MODE;

        private IWorldCameraService _cameraService;
        private IGameStateMachine _stateMachine;
        private IPersistentDataService _persistentData;
        private IPauseManager _pauseManager;
        private IAudioService _audioService;
        private IUserStateCommunicator  _communicator;
        private IAlertPopupProvider _alertPopupProvider;
        private IAgeStateService _ageState;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private IBeginGameManager _beginGameManager;

        [SerializeField] private Hud _hud;

        [SerializeField] private GameResultWindow _gameResultWindow;

        [SerializeField] private Battle _battle;

        private FoodGenerator _foodGenerator;

        private bool IsPaused => _pauseManager.IsPaused;
        private bool _scenarioInProcess;

        private bool IsInitialized { get; set; }

        public void Construct(
            IWorldCameraService cameraService,
            IRandomService random,
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,

            IAudioService audioService,
            IUserStateCommunicator communicator,
            IBeginGameManager beginGameManager,

            IBattleStateService battleState,
            
            IHeader header,
            
            IAgeStateService ageState,
            IUpgradesService upgradesService)
        {
            _cameraService = cameraService;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;

            _beginGameManager = beginGameManager;

            _ageState = ageState;
            
            
            InitializeCameras(cameraService);
            
            _gameResultWindow.Construct(_cameraService);
            
            _hud.Construct(
                _cameraService,
                _pauseManager,
                _alertPopupProvider,
                _audioService,
                battleState,
                _ageState);
            
            
            _foodGenerator = new FoodGenerator(upgradesService);

            _battle.Construct(
                battleState,
                _foodGenerator,
                _ageState);


            //TODO unregister if necessary
            _beginGameManager.Register(this);
            
            header.ShowWallet(persistentData);

            IsInitialized = true;
        }
        
        
        [Inject]
        private void InitializeFactories(
            IUnitFactory unitFactory,
            IBaseFactory baseFactory)
        {
            Factories = new GameObjectFactory[]
            {
               unitFactory as GameObjectFactory, 
               baseFactory as GameObjectFactory, 
            };
        }

        void IBeginGameHandler.BeginGame()
        {
            Cleanup();
            _hud.Show();
            _hud.QuitGame += GoToMainMenu;
            
            _foodGenerator.Init();
            _foodGenerator.FoodProgressUpdated += _hud.UpdateFoodFillAmount;
            _foodGenerator.FoodChanged += _hud.OnFoodChanged;
            _hud.OnFoodChanged(_foodGenerator.FoodAmount);
            
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
            if(!IsInitialized) return;

            if(IsPaused) return;
            
            if (_battle.ScenarioInProcess)
            {
                _battle.GameUpdate();
                _foodGenerator.GameUpdate();
            }
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