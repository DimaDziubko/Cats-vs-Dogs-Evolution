using _Game.Core._GameSaver;
using _Game.Core.Loading;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.PersistentData;
using _Game.GameModes._BattleMode.Scripts;
using _Game.Gameplay._BattleStateHandler;
using _Game.Gameplay._Race;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.Gameplay.GameResult.Scripts;
using _Game.UI._BattleUIController;
using Zenject;

namespace _Game.Gameplay.Battle.Scripts
{
    public interface IBattleMediator
    {
        bool BattleInProcess { get; }
        void StartBattle();
        void StopBattle();
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
        private readonly IPersistentDataService _persistentData;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IAudioService _audioService;
        private readonly IGameSaver _gameSaver;

        public bool BattleInProcess => _battle.BattleInProcess;

        public BattleMediator(
            BattleMode battleMode,
            Battle battle,
            BattleUIController uiController,
            BattleStateHandler battleStateHandler,
            IFoodGenerator foodGenerator,
            IUnitBuilder unitBuilder,
            RaceSelectionController raceSelectionController,
            GameResultHandler gameResultHandler,
            IPersistentDataService persistentData,
            IAudioService audioService)
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
            
            _persistentData = persistentData;

            _battleMode.SetMediator(this);
            _battle.SetMediator(this);
            _battleUIController.SetMediator(this);
            _battleStateHandler.SetMediator(this);
            _foodGenerator.SetMediator(this);
        }

        public void Initialize()
        {
            _battleMode.Init();
            //_raceSelectionController.Init();
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

        public void StopBattle()
        {
            _battleUIController.OnStopBattle();
            _battle.StopBattle();
            _unitBuilder.StopBuilder();
            _foodGenerator.StopGenerator();
        }

        public async void EndBattle(GameResultType result, bool wasExit = false)
        {
            if (!wasExit) _persistentData.AddCompletedBattle();
            
            var isConfirmed = await _gameResultHandler.ShowGameResultAndWaitForDecision(result, wasExit);
            if (isConfirmed)
            {
                if(result == GameResultType.Victory) PrepareNextBattle();
                _battleStateHandler.GoToMainMenu();
                ClearBattleMode();
            }
        }

        private void ClearBattleMode()
        {
            var clearGameOperation = new ClearGameOperation(_battleMode);
            _battleUIController.HideAndHandleLoadingOperation(clearGameOperation);
        }

        public void Reset() => 
            _battle.ResetSelf();

        private void PrepareNextBattle() => 
            _battle.PrepareNextBattle();


        public void Cleanup() => 
            _battle.Cleanup();
    }
}