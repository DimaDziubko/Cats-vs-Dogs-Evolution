using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Communication;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core._GameSaver
{
    public class GameSaver : IGameSaver, IDisposable
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IUserStateCommunicator _communicator;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IFoodBoostStateReadonly FoodBoostState => _persistentData.State.FoodBoost;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        private IBattleSpeedStateReadonly BattleSpeed => _persistentData.State.BattleSpeedState;

        private readonly float _debounceTime = 2.0f;
        private float _lastSaveTime;

        public GameSaver(
            IPersistentDataService persistentData, 
            IUserStateCommunicator communicator,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _persistentData = persistentData;
            _communicator = communicator;
            _featureUnlockSystem = featureUnlockSystem;
        }

        public void Init()
        {
            TutorialState.StepsCompletedChanged += OnStepCompleted;
            TimelineState.OpenedUnit += OnUnitOpened;
            TimelineState.UpgradeItemChanged += OnUpgradeItemChanged;
            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            RaceState.Changed += OnRaceChanged;
            BattleSpeed.IsNormalSpeedActiveChanged += OnBattleSpeedChanged;
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            TimelineState.OpenedUnit -= OnUnitOpened;
            TimelineState.UpgradeItemChanged -= OnUpgradeItemChanged;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            FoodBoostState.FoodBoostChanged -= OnFoodBoostChanged;
            RaceState.Changed -= OnRaceChanged;
            BattleSpeed.IsNormalSpeedActiveChanged -= OnBattleSpeedChanged;
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

        private void OnUpgradeItemChanged(UpgradeItemType _) => 
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