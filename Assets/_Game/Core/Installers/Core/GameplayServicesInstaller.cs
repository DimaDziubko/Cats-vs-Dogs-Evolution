using _Game.Core._DataPresenters._RaceChanger;
using _Game.Core._DataPresenters._TimelineInfoPresenter;
using _Game.Core._DataPresenters.Evolution;
using _Game.Core._DataPresenters.TimelineTravel;
using _Game.Core._DataPresenters.UnitUpgradePresenter;
using _Game.Core._RetentionChecker;
using _Game.Core._UpgradesChecker;
using _Game.Core.Ads;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using _Game.Core.DataPresenters._UpgradeItemPresenter;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.Upgrades;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Timer.Scripts;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Core.Pause.Scripts;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class GameplayServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUpgradesAvailabilityChecker();
            BindPauseManager();
            BindBattleSpeedManager();
            BindBeginGameManager();
            BindUpgradesCalculator();
            BindBoostsCalculator();
            BindUpgradesServices();
            BindEvolutionService();
            BindTimerService();
            BindAdsService();
            BindFoodBoostService();
            BindSpeedBoostService();
            BindBattleSpeedService();
            BindAnalyticsService();
            BindDTDAnalyticsService();
            BindTimelineNavigator();
            BindAgeNavigator();
            BindBattleNavigator();
            BindRaceChanger();
            BindRetentionChecker();
        }

        private void BindBoostsCalculator()
        {
            Container.BindInterfacesAndSelfTo<BoostsCalculator>().AsSingle();
        }

        private void BindRetentionChecker() => 
            Container.BindInterfacesAndSelfTo<RetentionChecker>().AsSingle();

        private void BindRaceChanger() => 
            Container.BindInterfacesAndSelfTo<RaceChanger>().AsSingle();

        private void BindDTDAnalyticsService() => 
            Container.BindInterfacesAndSelfTo<DTDAnalyticsService>().AsSingle();

        private void BindAnalyticsService() => 
            Container.BindInterfacesAndSelfTo<AnalyticsService>().AsSingle();

        private void BindUpgradesAvailabilityChecker() =>
            Container
                .BindInterfacesAndSelfTo<UpgradesAvailabilityChecker>()
                .AsSingle();

        private void BindPauseManager() =>
            Container.Bind<IPauseManager>()
                .To<PauseManager>()
                .AsSingle();

        private void BindBattleSpeedManager()
        {
            Container
                .BindInterfacesAndSelfTo<BattleSpeedManager>()
                .AsSingle();
        }

        private void BindBeginGameManager() =>
            Container
                .BindInterfacesAndSelfTo<BattleManager>()
                .AsSingle();

        private void BindUpgradesCalculator() =>
            Container
                .BindInterfacesAndSelfTo<UpgradeCalculator>()
                .AsSingle();

        private void BindUpgradesServices()
        {
            Container
                .BindInterfacesAndSelfTo<UpgradeItemPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<UnitUpgradesPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<TimelineInfoPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<TimelineTravelPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<StatsPopupPresenter>()
                .AsSingle();
        }

        private void BindEvolutionService() =>
            Container
                .BindInterfacesAndSelfTo<EvolutionPresenter>()
                .AsSingle();

        private void BindAdsService() =>
            Container
                .BindInterfacesAndSelfTo<CasAdsService>()
                .AsSingle();

        private void BindFoodBoostService() => 
            Container
                .BindInterfacesAndSelfTo<FoodBoostService>()
                .AsSingle();

        private void BindTimerService() =>
            Container
                .BindInterfacesAndSelfTo<TimerService>()
                .AsSingle();

        private void BindSpeedBoostService() =>
            Container
                .BindInterfacesAndSelfTo<SpeedBoostService>()
                .AsSingle();

        private void BindBattleSpeedService() =>
            Container
                .BindInterfacesAndSelfTo<BattleSpeedService>()
                .AsSingle();

        private void BindBattleNavigator() => 
            Container
                .BindInterfacesAndSelfTo<BattleNavigator>()
                .AsSingle();

        private void BindAgeNavigator() =>
            Container
                .BindInterfacesAndSelfTo<AgeNavigator>()
                .AsSingle();
        
        private void BindTimelineNavigator() =>
            Container
                .BindInterfacesAndSelfTo<TimelineNavigator>()
                .AsSingle();
    }
}