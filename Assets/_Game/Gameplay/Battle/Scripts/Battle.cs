using System;
using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.CoinCounter.Scripts;
using _Game.Gameplay.Scenario;
using _Game.Gameplay.Vfx.Factory;
using _Game.Utils.Disposable;
using UnityEngine;

namespace _Game.Gameplay.Battle.Scripts
{
    public class Battle : MonoBehaviour, IBaseDestructionHandler
    {
        [SerializeField] private Canvas _environmentCanvas;
        [SerializeField] private Transform _environmentAnchor;
        
        [SerializeField] private BattleField _battleField;

        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;

        private AudioClip _bGM;

        private IBaseDestructionManager _baseDestructionManager;
        private IBattleStateService _battleState;
        private IPauseManager _pauseManager;
        private IAudioService _audioService;

        private readonly Dictionary<string, Disposable<BattleEnvironment>> _environmentCache = new Dictionary<string, Disposable<BattleEnvironment>>();

        private int _currentBattle;
        private BattleEnvironment _currentBattleEnvironment;
        public bool BattleInProcess { get; private set; }

        public void Construct(
            IUnitFactory unitFactory,
            IBaseFactory baseFactory,
            IProjectileFactory projectileFactory,
            ICoinFactory coinFactory,
            IVfxFactory vfxFactory,
            IBattleStateService battleState,
            IWorldCameraService cameraService,
            IAgeStateService ageState,
            IPauseManager pauseManager,
            IAudioService audioService,
            IBaseDestructionManager baseDestructionManager,
            ICoinCounter coinCounter)
        {
            _battleField.Construct(
                unitFactory,
                baseFactory, 
                projectileFactory,
                vfxFactory,
                cameraService,
                pauseManager,
                ageState,
                audioService,
                coinFactory,
                baseDestructionManager,
                coinCounter);
            
            _environmentCanvas.worldCamera = cameraService.MainCamera;

            _audioService = audioService;
            
            _battleState = battleState;
            _pauseManager = pauseManager;

            _baseDestructionManager = baseDestructionManager;
            baseDestructionManager.Register(this);
        }

        public void Init()
        {
            _battleField.Init();
            
            _scenarioExecutor = new BattleScenarioExecutor();

            _battleState.BattlePrepared += UpdateBattle;

            UpdateBattle(_battleState.BattleData);
        }
        
        public void StartBattle()
        {
            BattleInProcess = true;
            _activeScenario = _scenarioExecutor.Begin(_battleField.UnitSpawner);
            _battleField.StartBattle();

            PlayBGM();
        }

        public void ResetSelf()
        {
            UpdateBattle(_battleState.BattleData);
            _battleField.ResetSelf();
        }

        public void PrepareNextBattle()
        {
            _battleState.OpenNextBattle(_currentBattle + 1);
        }

        public void GameUpdate()
        {
            if (BattleInProcess)
            {
                _activeScenario.Progress();
                _battleField.GameUpdate();
            }
        }

        public void Cleanup()
        {
            _battleField.Cleanup();
        }

        public void StopBattle()
        {
            StopBGM();
            BattleInProcess = false;
            _pauseManager.SetPaused(true);
        }

        private async void UpdateBattle(BattleData data)
        {
            _currentBattle = data.Battle;
            _bGM = data.BGM;
            _battleField.UpdateBase(Faction.Enemy);
            _scenarioExecutor.UpdateScenario(data.Scenario);

            if (_currentBattleEnvironment != null)
            {
                _currentBattleEnvironment.Hide();
            }
            
            if (_environmentCache.ContainsKey(data.EnvironmentKey))
            {
                var battleEnvironment = _environmentCache[data.EnvironmentKey];
                battleEnvironment.Value.Show();
                _currentBattleEnvironment = battleEnvironment.Value;
            }
            else
            {
                LocalAssetLoader assetLoader = new LocalAssetLoader();
                Disposable<BattleEnvironment> environment =
                    await assetLoader.LoadDisposable<BattleEnvironment>(data.EnvironmentKey, _environmentAnchor);
                _environmentCache.Add(data.EnvironmentKey, environment);
            }
        }

        private void OnDisable()
        {
            //TODO Check place
            _battleState.BattlePrepared -= UpdateBattle;
            _baseDestructionManager.UnRegister(this);
        }

        private void PlayBGM()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Play(_bGM);
            }
        }

        private void StopBGM()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Stop();
            }
        }

        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            StopBattle();
        }

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            
        }
    }
}