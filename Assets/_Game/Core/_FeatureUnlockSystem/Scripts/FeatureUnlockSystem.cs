using System;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Utils;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem
    {
        private readonly IPersistentDataService _persistentData;
        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private IBattleStatisticsReadonly BattleStatisticsState => _persistentData.State.BattleStatistics;
        
        public FeatureUnlockSystem(IPersistentDataService persistentData)
        {
            _persistentData = persistentData;
        }

        public bool IsFeatureUnlocked(IFeature feature)
        {
            switch (feature.Feature)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
            }
        }

        private bool GetTresholdForFoodBoost()
        {
            return TutorialState.StepsCompleted >= Constants.TutorialSteps.FOOD_UPGRADE_ITEM &&
                   BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.FOOD_BOOST;
        }

        private bool GetTresholdForPause()
        {
            return TutorialState.StepsCompleted > Constants.TutorialSteps.UNIT_BUILDER_BUTTON &&
                   BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.PAUSE;
        }

        private bool GetTresholdForUpgradesWindow()
        {
            return TutorialState.StepsCompleted >= Constants.TutorialSteps.UPGRADES_WINDOW &&
                   BattleStatisticsState.BattlesCompleted >= Constants.FeatureCompletedBattleThresholds.UPGRADES_WINDOW;
        }
    }

    public interface IFeatureUnlockSystem
    {
        bool IsFeatureUnlocked(IFeature feature);
    }
}