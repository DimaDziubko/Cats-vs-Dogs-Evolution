using System.Threading;
using _Game.Core._SystemUpdate;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Scenario;
using _Game.UI._Environment;
using UnityEngine;

namespace _Game.Gameplay.Battle.Scripts
{
    public class Battle : IGameUpdate
    {
        private readonly BattleField _battleField;
        private readonly EnvironmentController _environmentController;

        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;

        private AudioClip _bGM;

        private readonly IBattleStateService _battleState;
        private readonly IPauseManager _pauseManager;
        private readonly IAudioService _audioService;
        private readonly ISystemUpdate _systemUpdate;
        private readonly ICoinCounter _coinCounter;

        private int _currentBattle;
        public bool BattleInProcess { get; private set; }
        private bool IsPaused => _pauseManager.IsPaused;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public Battle(
            IBattleStateService battleState,
            IPauseManager pauseManager,
            IAudioService audioService,
            ISystemUpdate systemUpdate,
            ICoinCounter coinCounter,
            BattleField battleField,
            EnvironmentController environmentController)
        {
            _systemUpdate = systemUpdate;
            _battleField = battleField;
            _coinCounter = coinCounter;
            
            _audioService = audioService;
            
            _battleState = battleState;
            _pauseManager = pauseManager;
            _environmentController = environmentController;
        }

        public void Init()
        {
            _battleField.Init();
            
            _scenarioExecutor = new BattleScenarioExecutor();

            _battleState.BattlePrepared += UpdateBattle;

            UpdateBattle(_battleState.BattleData);
            _systemUpdate.Register(this);
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
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
        }

        public void PrepareNextBattle() => 
            _battleState.OpenNextBattle(_currentBattle + 1);

        void IGameUpdate.GameUpdate()
        {
            if(IsPaused) return;
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

        private void UpdateBattle(BattleData data)
        {
            _environmentController.ShowEnvironment(data.EnvironmentData);
            _currentBattle = data.Battle;
            _bGM = data.BGM;
            _battleField.UpdateBase(Faction.Enemy);
            _scenarioExecutor.UpdateScenario(data.Scenario);
            _coinCounter.MaxCoinsPerBattle = data.MaxCoinsPerBattle;
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

        public void SetMediator(IBattleMediator battleMediator)
        {
            
        }
    }
}