using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._GameListenerComposite;
using _Game.Core.Communication;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core._GameSaver
{
    public class GameSaver : 
        IGameSaver, 
        IDisposable,
        IStopBattleListener
    {
        private readonly IUserContainer _userContainer;
        private readonly IUserStateCommunicator _communicator;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;

        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IFoodBoostStateReadonly FoodBoostState => _userContainer.State.FoodBoost;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IBattleSpeedStateReadonly BattleSpeed => _userContainer.State.BattleSpeedState;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;
        private IDailyTasksStateReadonly DailyTasksState => _userContainer.State.DailyTasksState;

        private readonly float _debounceTime = 2.0f;
        private float _lastSaveTime;

        public GameSaver(
            IUserContainer userContainer, 
            IUserStateCommunicator communicator,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _communicator = communicator;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPreInitialization += Init;
        }

        public void Init()
        {
            TutorialState.StepsCompletedChanged += OnStepCompleted;
            TimelineState.OpenedUnit += OnUnitOpened;
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemLevelChanged;
            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            RaceState.Changed += OnRaceChanged;
            BattleSpeed.IsNormalSpeedActiveChanged += OnBattleSpeedChanged;
            BattleSpeed.PermanentSpeedChanged += OnPermanentBattleSpeedChanged;
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            Purchases.Changed += OnPurchasesChanged;
            DailyTasksState.TaskCompletedChanged += OnTaskCompleted;
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            TimelineState.OpenedUnit -= OnUnitOpened;
            TimelineState.UpgradeItemLevelChanged -= OnUpgradeItemLevelChanged;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            FoodBoostState.FoodBoostChanged -= OnFoodBoostChanged;
            RaceState.Changed -= OnRaceChanged;
            BattleSpeed.IsNormalSpeedActiveChanged -= OnBattleSpeedChanged;
            BattleSpeed.PermanentSpeedChanged -= OnPermanentBattleSpeedChanged;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            Purchases.Changed -= OnPurchasesChanged;
            _gameInitializer.OnPreInitialization -= Init;
            DailyTasksState.TaskCompletedChanged -= OnTaskCompleted;
        }

        private void OnTaskCompleted() => 
            SaveGame();

        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source) => 
            SaveGame();

        private void OnPurchasesChanged() => 
            SaveGame();

        private void OnPermanentBattleSpeedChanged(int id) => 
            SaveGame();

        private void OnBattleSpeedChanged(bool _) => 
            SaveGame();

        private void OnNextBattleOpened() => 
            SaveGame();

        private void OnRaceChanged() => 
            SaveGame();
        
        private void OnFoodBoostChanged() => 
            SaveGame();

        private void OnFeatureUnlocked(Feature _) => 
            SaveGame();

        private void OnNextTimelineOpened() => 
            SaveGame();

        private void OnNextAgeOpened() => 
            SaveGame();

        private void OnUpgradeItemLevelChanged(UpgradeItemType _, int __) => 
            DebounceSaveGame();

        private void OnUnitOpened(UnitType _) => 
            SaveGame();
        
        private void OnStepCompleted(int step) => 
            SaveGame();

        private void DebounceSaveGame()
        {
            if (Time.time - _lastSaveTime >= _debounceTime)
            {
                SaveGame();
                _lastSaveTime = Time.time;
            }
        }
        
        private void SaveGame() => 
            _communicator.SaveUserState(_userContainer.State);

        void IStopBattleListener.OnStopBattle()
        {
            SaveGame();
        }
    }
}