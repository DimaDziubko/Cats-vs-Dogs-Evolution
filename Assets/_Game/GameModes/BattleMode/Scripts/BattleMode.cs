using System;
using System.Collections.Generic;
using _Game.Common;
using _Game.Core.Communication;
using _Game.Core.Factory;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.Gameplay.CoinCounter.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.Gameplay.GameResult.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Hud;
using _Game.Utils;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.GameModes.BattleMode.Scripts
{
    public class BattleMode : MonoBehaviour, IGameModeCleaner, IBattleLauncher, IBaseDestructionHandler
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
        private IHeader _header;
        private IUnitBuilder _unitBuilder;
        private IFoodGenerator _foodGenerator;
        private IGameResultWindowProvider _gameResultWindowProvider;
        private ILoadingScreenProvider _loadingScreenProvider;
        private ICoinCounter _coinCounter;
        private IRewardAnimator _rewardAnimator;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currencies => _persistentData.State.Currencies;

        private IBattleLaunchManager _battleLaunchManager;

        [SerializeField] private Hud _hud;

        [SerializeField] private Battle _battle;


        private bool IsPaused => _pauseManager.IsPaused;
        private bool _scenarioInProcess;

        private bool IsInitialized { get; set; }
        
        #region Ctor
        
        [Inject]
        public void Construct(IWorldCameraService cameraService,
            IRandomService random,
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IBattleLaunchManager battleLaunchManager,
            IBattleStateService battleState,
            IHeader header,
            IEconomyUpgradesService economyUpgradesService,
            IUnitFactory unitFactory,
            IBaseFactory baseFactory,
            IProjectileFactory projectileFactory,
            ICoinFactory coinFactory,
            IUnitBuilder unitBuilder,
            IFoodGenerator foodGenerator,
            IGameResultWindowProvider gameResultWindowProvider,
            ILoadingScreenProvider loadingScreenProvider,
            IAgeStateService ageState,
            ICoinCounter coinCounter,
            IRewardAnimator rewardAnimator,
            IVfxFactory vfxFactory,
            IBaseDestructionManager baseDestructionManager)
        {
            _cameraService = cameraService;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _pauseManager = pauseManager;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;

            _battleLaunchManager = battleLaunchManager;
            
            _header = header;

            _unitBuilder = unitBuilder;
            _foodGenerator = foodGenerator;
            _gameResultWindowProvider = gameResultWindowProvider;
            _loadingScreenProvider = loadingScreenProvider;

            _coinCounter = coinCounter;
            _rewardAnimator = rewardAnimator;
            
            _hud.Construct(
                _cameraService,
                _pauseManager,
                _alertPopupProvider,
                _audioService);

            _battle.Construct(
                unitFactory,
                baseFactory,
                projectileFactory,
                coinFactory,
                vfxFactory,
                battleState,
                cameraService,
                ageState,
                pauseManager,
                audioService,
                baseDestructionManager,
                coinCounter);
            
            Factories = new GameObjectFactory[]
            {
               unitFactory as GameObjectFactory, 
               baseFactory as GameObjectFactory, 
               projectileFactory as GameObjectFactory, 
               coinFactory as GameObjectFactory, 
               vfxFactory as GameObjectFactory, 
            };
            
            baseDestructionManager.Register(this);
        }
        
        #endregion
        
        public void Init()
        {
            _battleLaunchManager.Register(this);
            _header.ShowWallet(
                Currencies,
                _cameraService);
            
            _battle.Init();
            _foodGenerator.Init();
            
            IsInitialized = true;
        }
        
        void IBattleLauncher.LaunchBattle()
        {
            Cleanup();
            
            _hud.Show();
            _hud.QuitGame += GoToMainMenu;

            _foodGenerator.StartGenerator();
            _unitBuilder.StartBuilder();
            
            _coinCounter.Changed += _hud.OnCoinsChanged;
            _hud.OnCoinsChanged(_coinCounter.Coins);


            _battle.StartBattle();
            
            _stateMachine.Enter<GameLoopState>();
        }
        
        public void Cleanup()
        {
            _coinCounter.Changed -= _hud.OnCoinsChanged;
            _hud.QuitGame -= GoToMainMenu;
            _hud.Hide();
            _battle.Cleanup();
        }

        public void ResetGame()
        {
            _battle.ResetSelf();
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
        }

        private async void BattleCompleted(GameResultType type)
        {
            StopBattle();
            
            var popup = await _gameResultWindowProvider.Load();

            var isConfirmed = await popup.Value.ShowAndAwaitForExit(_coinCounter.Coins, type);
            if (isConfirmed)
            {
                GoToMainMenu();
                if (type == GameResultType.Victory) _battle.PrepareNextBattle();
            }
            popup.Dispose();

            
            SaveGame();
        }

        private void StopBattle()
        {
            _foodGenerator.StopGenerator();
            _unitBuilder.StopBuilder();
            _battle.StopBattle();
        }


        private void Update()
        {
            if(!IsInitialized) return;

            if(IsPaused) return;

            if (_battle.BattleInProcess)
            {
                _battle.GameUpdate();
                _foodGenerator.GameUpdate();
            }
        }

        private void SaveGame() => _communicator.SaveUserState(_persistentData.State);
        
        private void GoToMainMenu()
        {
            var clearingOperation = new ClearGameOperation(this);
            
            _loadingScreenProvider.LoadAndDestroy(clearingOperation).Forget();

            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
        }

        private void OnLoadingCompleted()
        {
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
            _stateMachine.Enter<MenuState>();

            _rewardAnimator.PlayCoins(_header.CoinsWalletWorldPosition);

            if (_coinCounter.Coins > 0)
            {
                _persistentData.AddCoins(_coinCounter.Coins);
                _coinCounter.Cleanup();
            }
        }

        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base) { }

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            switch (faction)
            {
                case Faction.Player:
                    BattleCompleted(GameResultType.Defeat);
                    break;
                case Faction.Enemy:
                    BattleCompleted(GameResultType.Victory);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        //TODO Delete
        [Button]
        public void CoinsTest()
        {
            _persistentData.AddCoins(1000);
        }
    }
}