using System.Collections.Generic;
using _Game.Core.Navigation.Battle;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.GameModes._BattleMode.Scripts;
using _Game.Gameplay._BattleStateHandler;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._BattleUIController;
using _Game.UI._Environment;
using Assets._Game.Core._GameSaver;
using Assets._Game.Core.Loading;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Race;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay.Food.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Zenject;

namespace _Game.Gameplay._Battle.Scripts
{
    public interface IBattleMediator
    {
        bool BattleInProcess { get; }
        void StartBattle();
        void StopBattle();
        void OnWaveChanged(int currentWave, int wavesCount);
        void EndBattle(GameResultType result, bool wasExit = false);
        void Reset();
        void Cleanup();
    }

    public class BattleMediator : IBattleMediator, IInitializable
    {
        private readonly BattleMode _battleMode;
        private readonly Battle _battle;
        private readonly BattleUIController _battleUIController;
        private readonly BattleStateHandler _battleStateHandler;
        private readonly IUnitBuilder _unitBuilder;
        private readonly IFoodGenerator _foodGenerator;
        private readonly RaceSelectionController _raceSelectionController;
        private readonly GameResultHandler _gameResultHandler;
        private readonly IUserContainer _userContainer;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IAudioService _audioService;
        private readonly IGameSaver _gameSaver;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ISoundService _soundService;
        private readonly EnvironmentController _environmentController;

        public bool BattleInProcess => _battle.BattleInProcess;

        public BattleMediator(
            BattleMode battleMode,
            Battle battle,
            BattleUIController uiController,
            BattleStateHandler battleStateHandler,
            EnvironmentController environmentController,
            IFoodGenerator foodGenerator,
            IUnitBuilder unitBuilder,
            RaceSelectionController raceSelectionController,
            GameResultHandler gameResultHandler,
            IUserContainer userContainer,
            IAudioService audioService,
            IBattleNavigator battleNavigator,
            ISoundService soundService)
        {
            _battleMode = battleMode;
            _battle = battle;
            _battleUIController = uiController;
            _battleStateHandler = battleStateHandler;
            _unitBuilder = unitBuilder;
            _foodGenerator = foodGenerator;
            _raceSelectionController = raceSelectionController;
            _gameResultHandler = gameResultHandler;
            _audioService = audioService;
            _battleNavigator = battleNavigator;
            _soundService = soundService;
            _userContainer = userContainer;
            _environmentController = environmentController;

            _battleMode.SetMediator(this);
            _battle.SetMediator(this);
            _battleUIController.SetMediator(this);
            _battleStateHandler.SetMediator(this);
            _foodGenerator.SetMediator(this);
        }

        public void Initialize()
        {
            _battleMode.Init();
            _environmentController.Init();
            _raceSelectionController.Init();
            _battle.Init();
            _foodGenerator.Init();
        }
        
        public void StartBattle()
        {
            _battle.StartBattle();
            _battleUIController.OnStartBattle();
            _unitBuilder.StartBuilder();
            _foodGenerator.StartGenerator();
            _battleStateHandler.HandleStart();
            _audioService.PlayStartBattleSound();
        }

        public void OnWaveChanged(int currentWave, int wavesCount)
        {
            _battleUIController.UpdateWave(currentWave, wavesCount);
        }

        public void StopBattle()
        {
            _battleUIController.OnStopBattle();
            _battle.StopBattle();
            _unitBuilder.StopBuilder();
            _foodGenerator.StopGenerator();
        }

        public async void EndBattle(GameResultType result, bool wasExit = false)
        {
            if (!wasExit) _userContainer.AnalyticsStateHandler.AddCompletedBattle();
            
            var isConfirmed = await _gameResultHandler.ShowGameResultAndWaitForDecision(result, wasExit);
            
            if (isConfirmed)
            {
                if(result == GameResultType.Victory) _battleNavigator.OpenNextBattle();
                GoToMainMenu();
            }
        }

        private void GoToMainMenu()
        {
            Queue <ILoadingOperation> loadingOperations = new Queue<ILoadingOperation>();
            loadingOperations.Enqueue(new ClearGameOperation(
                _battleMode,
                _soundService));
            _battleUIController.HideCoinCounter();
            _battleUIController.ShowRewardCoinsAfterLoading();
            _battleStateHandler.GoToMainMenu(loadingOperations);
        }
        
        public void Reset() => 
            _battle.ResetSelf();
        
        
        public void Cleanup() => 
            _battle.Cleanup();
    }
}