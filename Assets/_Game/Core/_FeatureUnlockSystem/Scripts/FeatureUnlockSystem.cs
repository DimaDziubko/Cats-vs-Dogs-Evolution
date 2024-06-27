using System;
using _Game.Core._GameInitializer;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Utils;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable
    {
        private const int BATTLE_SPEED_AGE_TRESHOLD = 1;
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
            TimelineState.AgeId  >= BATTLE_SPEED_AGE_TRESHOLD;

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