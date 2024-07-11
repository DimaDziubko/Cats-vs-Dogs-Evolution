using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.Analytics;
using Assets._Game.Core._RetentionChecker;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.Ads;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.DataPresenters._TimelineInfoPresenter;
using Assets._Game.Core.DataPresenters._UpgradeItemPresenter;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.DataPresenters.TimelineTravel;
using Assets._Game.Core.DataPresenters.UnitUpgradePresenter;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services._BattleSpeedService._Scripts;
using Assets._Game.Core.Services._FoodBoostService.Scripts;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.Services.Upgrades;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay._Timer.Scripts;
using Assets._Game.Gameplay.BattleLauncher;
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
            BindUpgradesServices();
            BindEvolutionService();
            BindAdsService();
            BindFoodBoostService();
            BindTimerService();
            BindBattleSpeedService();
            BindAnalyticsService();
            BindDTDAnalyticsService();
            BindTimelineNavigator();
            BindAgeNavigator();
            BindBattleNavigator();
            BindRaceChanger();
            BindRetentionChecker();
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
                .BindInterfacesAndSelfTo<BattleLaunchManager>()
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
        }

        private void BindEvolutionService() =>
            Container
                .BindInterfacesAndSelfTo<EvolutionPresenter>()
                .AsSingle();

        private void BindAdsService() =>
            Container
                .Bind<IAdsService>().To<CasAdsService>()
                .AsSingle()
                .NonLazy();

        private void BindFoodBoostService() => 
            Container
                .BindInterfacesAndSelfTo<FoodBoostService>()
                .AsSingle();

        private void BindTimerService() =>
            Container
                .BindInterfacesAndSelfTo<TimerService>()
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