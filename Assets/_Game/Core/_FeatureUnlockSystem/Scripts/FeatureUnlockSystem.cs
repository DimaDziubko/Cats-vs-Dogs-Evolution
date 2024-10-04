﻿using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Assets._Game.Core.UserState;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable
    {
        private const int BATTLE_SPEED_AGE_TRESHOLD = 1;
        private const int PAUSE_AGE_TRESHOLD = 1;
        public event Action<Feature> FeatureUnlocked;

        private readonly IUserContainer _persistentData;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private readonly Dictionary<Feature, bool> _featureUnlockState = new Dictionary<Feature, bool>();
        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IBattleStatisticsReadonly BattleStatisticsState => _persistentData.State.BattleStatistics;

        public FeatureUnlockSystem(
            IUserContainer persistentData,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameInitializer = gameInitializer;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            foreach (Feature feature in Enum.GetValues(typeof(Feature)))
            {
                _featureUnlockState[feature] = CheckInitialUnlockState(feature);
            }

            TutorialState.StepsCompletedChanged += OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged += OnBattleStatisticsChanged;
        }

        void IDisposable.Dispose()
        {
            TutorialState.StepsCompletedChanged -= OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged -= OnBattleStatisticsChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        public bool IsFeatureUnlocked(Feature feature)
        {
            _logger.Log($"Ask for feature {feature}");
            return _featureUnlockState.TryGetValue(feature, out bool isUnlocked) && isUnlocked;
        }

        private bool CheckInitialUnlockState(Feature feature)
        {
            switch (feature)
            {
                case Feature.None:
                    return true;
                case Feature.Pause:
                    return GetTresholdForPause();
                case Feature.FoodBoost:
                    return GetTresholdForFoodBoost();
                case Feature.AlwaysUnlocked:
                    return true;
                case Feature.UpgradesScreen:
                    return GetTresholdForUpgradesScreen();
                case Feature.EvolutionScreen:
                    return GetTresholdForEvolutionScreen();
                case Feature.BattleSpeed:
                    return GetTresholdForBattleSpeed();
                case Feature.X2:
                    return GetTresholdForX2();
                case Feature.Shop:
                    return GetTresholdForShop();
                case Feature.DailyTask:
                    return GetTresholdForDailyTask();
                default:
                    return false;
            }
        }

        private void OnBattleStatisticsChanged() => 
            CheckForFeaturesUnlock();

        private void OnTutorialStepCompleted(int step) => 
            CheckForFeaturesUnlock();

        private void CheckForFeaturesUnlock()
        {
            CheckForEvolutionFeatureUnlock();
            CheckForBattleSpeedFeatureUnlock();
            CheckForDailyTaskUnlock();
            CheckForUpgradesFeatureUnlock();
            CheckForFoodBoostFeatureUnlock();
            CheckForShopFeatureUnlock();
            CheckForX2FeatureUnlock();
            CheckForPauseFeatureUnlock();
        }

        private void CheckForDailyTaskUnlock()
        {
            if (!IsFeatureUnlocked(Feature.DailyTask) && GetTresholdForDailyTask())
            {
                _featureUnlockState[Feature.DailyTask] = true;
                FeatureUnlocked?.Invoke(Feature.DailyTask);
            }
        }

        private void CheckForBattleSpeedFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.BattleSpeed) && GetTresholdForBattleSpeed())
            {
                //Disable
                _featureUnlockState[Feature.BattleSpeed] = false;
                // _featureUnlockState[Feature.BattleSpeed] = true;
                // FeatureUnlocked?.Invoke(Feature.BattleSpeed);
            }
        }

        private void CheckForEvolutionFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.EvolutionScreen) && GetTresholdForEvolutionScreen())
            {
                _featureUnlockState[Feature.EvolutionScreen] = true;
                FeatureUnlocked?.Invoke(Feature.EvolutionScreen);
            }
        }

        private void CheckForShopFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.Shop) && GetTresholdForShop())
            {
                _featureUnlockState[Feature.Shop] = true;
                FeatureUnlocked?.Invoke(Feature.Shop);
            }
        }

        private void CheckForUpgradesFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.UpgradesScreen) && GetTresholdForUpgradesScreen())
            {
                _featureUnlockState[Feature.UpgradesScreen] = true;
                FeatureUnlocked?.Invoke(Feature.UpgradesScreen);
            }
        }

        private void CheckForFoodBoostFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.FoodBoost) && GetTresholdForFoodBoost())
            {
                _featureUnlockState[Feature.FoodBoost] = true;
                FeatureUnlocked?.Invoke(Feature.FoodBoost);
            }
        }

        private void CheckForX2FeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.X2) && GetTresholdForX2())
            {
                _featureUnlockState[Feature.X2] = true;
                FeatureUnlocked?.Invoke(Feature.X2);
            }
        }

        private void CheckForPauseFeatureUnlock()
        {
            if (!IsFeatureUnlocked(Feature.Pause) && GetTresholdForPause())
            {
                _featureUnlockState[Feature.Pause] = true;
                FeatureUnlocked?.Invoke(Feature.Pause);
            }
        }

        public bool IsFeatureUnlocked(IFeature feature) => 
            IsFeatureUnlocked(feature.Feature);

        private bool GetTresholdForDailyTask() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.EVOLUTION_SCREEN &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.SHOP;

        private bool GetTresholdForShop() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.EVOLUTION_SCREEN &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.SHOP;

        private bool GetTresholdForX2() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.FOOD_UPGRADE_ITEM &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.X2;

        private bool GetTresholdForBattleSpeed() => TimelineState.TimelineId > 0 ||
            TimelineState.AgeId >= BATTLE_SPEED_AGE_TRESHOLD;

        private bool GetTresholdForEvolutionScreen() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.EVOLUTION_SCREEN &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.EVOLUTION_SCREEN;

        private bool GetTresholdForFoodBoost() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.FOOD_UPGRADE_ITEM &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.FOOD_BOOST;

        private bool GetTresholdForPause() =>
            TimelineState.AgeId >= PAUSE_AGE_TRESHOLD;

        private bool GetTresholdForUpgradesScreen() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.UPGRADES_SCREEN &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.UPGRADES_SCREEN;
    }

}