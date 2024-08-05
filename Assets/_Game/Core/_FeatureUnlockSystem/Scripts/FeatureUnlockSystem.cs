using System;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.UserState;
using Assets._Game.Utils;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable
    {
        private const int BATTLE_SPEED_AGE_TRESHOLD = 1;
        private const int PAUSE_AGE_TRESHOLD = 1;
        public event Action<Feature> FeatureUnlocked;

        private readonly IUserContainer _persistentData;
        private readonly IGameInitializer _gameInitializer;

        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IBattleStatisticsReadonly BattleStatisticsState => _persistentData.State.BattleStatistics;

        public FeatureUnlockSystem(
            IUserContainer persistentData,
            IGameInitializer gameInitializer)
        {
            _persistentData = persistentData;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TutorialState.StepsCompletedChanged += OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged += OnBattleStatisticsChanged;
        }

        void IDisposable.Dispose()
        {
            TutorialState.StepsCompletedChanged -= OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged -= OnBattleStatisticsChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnBattleStatisticsChanged() => 
            CheckForFeaturesUnlock();

        private void OnTutorialStepCompleted(int step) => 
            CheckForFeaturesUnlock();

        private void CheckForFeaturesUnlock()
        {
            CheckForEvolutionFeatureUnlock();
            CheckForBattleSpeedFeatureUnlock();
        }

        private void CheckForBattleSpeedFeatureUnlock()
        {
            if (GetTresholdForBattleSpeed())
            {
                FeatureUnlocked?.Invoke(Feature.BattleSpeed);
            }
        }

        private void CheckForEvolutionFeatureUnlock()
        {
            if (GetTresholdForEvolutionScreen())
            {
                FeatureUnlocked?.Invoke(Feature.EvolutionScreen);
            }
        }

        public bool IsFeatureUnlocked(IFeature feature) => 
            IsFeatureUnlocked(feature.Feature);

        public bool IsFeatureUnlocked(Feature feature)
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
                default:
                    return false;
            }
        }

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