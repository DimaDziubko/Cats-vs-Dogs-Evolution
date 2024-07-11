using System;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core.Communication;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace Assets._Game.Core._GameSaver
{
    public class GameSaver : IGameSaver, IDisposable
    {
        private readonly IUserContainer _persistentData;
        private readonly IUserStateCommunicator _communicator;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;

        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IFoodBoostStateReadonly FoodBoostState => _persistentData.State.FoodBoost;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        private IBattleSpeedStateReadonly BattleSpeed => _persistentData.State.BattleSpeedState;
        private IUserCurrenciesStateReadonly Currencies => _persistentData.State.Currencies;

        private readonly float _debounceTime = 2.0f;
        private float _lastSaveTime;

        public GameSaver(
            IUserContainer persistentData, 
            IUserStateCommunicator communicator,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer)
        {
            _persistentData = persistentData;
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
            _gameInitializer.OnPreInitialization -= Init;
        }

        private void OnBattleSpeedChanged(bool _) => 
            SaveGame();

        private void OnNextBattleOpened() => 
            SaveGame();

        private void OnRaceChanged() => 
            SaveGame();

        public void OnBattleStopped() => 
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
            _communicator.SaveUserState(_persistentData.State);
    }
}