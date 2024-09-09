using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Assets._Game.Core.UserState;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable, ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        public event Action<Feature> FeatureUnlocked;

        private const int BATTLE_SPEED_AGE_THRESHOLD = 1;
        private const int PAUSE_AGE_THRESHOLD = 1;

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
            return feature switch
            {
                Feature.None => true,
                Feature.Pause => GetThresholdFor(GetThresholdForPause),
                Feature.FoodBoost => GetThresholdFor(GetThresholdForFoodBoost),
                Feature.AlwaysUnlocked => true,
                Feature.UpgradesScreen => GetThresholdFor(GetThresholdForUpgradesScreen),
                Feature.EvolutionScreen => GetThresholdFor(GetThresholdForEvolutionScreen),
                Feature.BattleSpeed => GetThresholdFor(GetThresholdForBattleSpeed),
                Feature.X2 => GetThresholdFor(GetThresholdForX2),
                Feature.Shop => GetThresholdFor(GetThresholdForShop),
                Feature.DailyTask => GetThresholdFor(GetThresholdForDailyTask),
                Feature.Cards => GetThresholdFor(GetThresholdForCards),
                _ => false
            };
        }

        private bool GetThresholdFor(Func<bool> thresholdFunc)
        {
            return thresholdFunc();
        }

        private bool GetThresholdForCards() => true;

        private bool GetThresholdForDailyTask() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepThresholds.EVOLUTION_SCREEN &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.SHOP;

        private bool GetThresholdForShop() => GetThresholdForDailyTask();

        private bool GetThresholdForX2() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepThresholds.FOOD_UPGRADE_ITEM &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.X2;

        private bool GetThresholdForBattleSpeed() =>
            TimelineState.TimelineId > 0 || TimelineState.AgeId >= BATTLE_SPEED_AGE_THRESHOLD;

        private bool GetThresholdForEvolutionScreen() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepThresholds.EVOLUTION_SCREEN &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.EVOLUTION_SCREEN;

        private bool GetThresholdForFoodBoost() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepThresholds.FOOD_UPGRADE_ITEM &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.FOOD_BOOST;

        private bool GetThresholdForPause() => TimelineState.AgeId >= PAUSE_AGE_THRESHOLD;

        private bool GetThresholdForUpgradesScreen() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepThresholds.UPGRADES_SCREEN &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.UPGRADES_SCREEN;

        private void OnBattleStatisticsChanged() => CheckForFeaturesUnlock();
        private void OnTutorialStepCompleted(int step) => CheckForFeaturesUnlock();

        private void CheckForFeaturesUnlock()
        {
            CheckAndUnlockFeature(Feature.Cards, GetThresholdForCards);
            CheckAndUnlockFeature(Feature.DailyTask, GetThresholdForDailyTask);
            CheckAndUnlockFeature(Feature.BattleSpeed, GetThresholdForBattleSpeed);
            CheckAndUnlockFeature(Feature.EvolutionScreen, GetThresholdForEvolutionScreen);
            CheckAndUnlockFeature(Feature.Shop, GetThresholdForShop);
            CheckAndUnlockFeature(Feature.UpgradesScreen, GetThresholdForUpgradesScreen);
            CheckAndUnlockFeature(Feature.FoodBoost, GetThresholdForFoodBoost);
            CheckAndUnlockFeature(Feature.X2, GetThresholdForX2);
            CheckAndUnlockFeature(Feature.Pause, GetThresholdForPause);
        }

        private void CheckAndUnlockFeature(Feature feature, Func<bool> getThresholdFunc)
        {
            if (!IsFeatureUnlocked(feature) && getThresholdFunc())
            {
                _featureUnlockState[feature] = true;
                FeatureUnlocked?.Invoke(feature);
                SaveGameRequested?.Invoke(false);
            }
        }

        public bool IsFeatureUnlocked(IFeature feature) => IsFeatureUnlocked(feature.Feature);
    }
}