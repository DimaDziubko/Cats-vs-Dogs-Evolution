using System;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Utils;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public interface IFeatureUnlockSystem
    {
        void Init();
        bool IsFeatureUnlocked(IFeature feature);
        bool IsFeatureUnlocked(Feature feature);
        event Action<Feature> FeatureUnlocked;
    }

    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable
    {
        public event Action<Feature> FeatureUnlocked;

        private readonly IPersistentDataService _persistentData;

        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;

        private IBattleStatisticsReadonly BattleStatisticsState => _persistentData.State.BattleStatistics;

        public FeatureUnlockSystem(IPersistentDataService persistentData) => _persistentData = persistentData;

        public void Init()
        {
            TutorialState.StepsCompletedChanged += OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged += OnBattleStatisticsChanged;
        }

        public void Dispose()
        {
            TutorialState.StepsCompletedChanged -= OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged -= OnBattleStatisticsChanged;
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
            if (GetTresholdForEvolutionWindow())
            {
                FeatureUnlocked?.Invoke(Feature.EvolutionWindow);
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
                case Feature.UpgradesWindow:
                    return GetTresholdForUpgradesWindow();
                case Feature.EvolutionWindow:
                    return GetTresholdForEvolutionWindow();
                case Feature.BattleSpeed:
                    return GetTresholdForBattleSpeed();
                case Feature.X2:
                    return GetTresholdForX2();
                default:
                    return false;
            }
        }

        private bool GetTresholdForX2() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.FOOD_UPGRADE_ITEM &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.X2;
        

        private bool GetTresholdForBattleSpeed() =>
            TutorialState.StepsCompleted > Constants.TutorialStepTreshold.EVOLUTION_WINDOW &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.BATTLE_SPEED;

        private bool GetTresholdForEvolutionWindow() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.EVOLUTION_WINDOW &&
                BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.EVOLUTION_WINDOW;

        private bool GetTresholdForFoodBoost() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.FOOD_UPGRADE_ITEM &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.FOOD_BOOST;

        private bool GetTresholdForPause() =>
            TutorialState.StepsCompleted > Constants.TutorialStepTreshold.UNIT_BUILDER_BUTTON &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.PAUSE;

        private bool GetTresholdForUpgradesWindow() =>
            TutorialState.StepsCompleted >= Constants.TutorialStepTreshold.UPGRADES_WINDOW &&
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.UPGRADES_WINDOW;
    }
}