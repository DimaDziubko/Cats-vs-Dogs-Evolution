using _Game.Core._RetentionChecker;
using _Game.Core._UpgradesChecker;
using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Core.DataPresenters._RaceChanger;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using _Game.Core.DataPresenters._UpgradeItemPresenter;
using _Game.Core.DataPresenters.Evolution;
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
using _Game.Gameplay._Timer.Scripts;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Core.Pause.Scripts;
using Zenject;
using UnityEngine;
using MAXHelper;

namespace _Game.Core.Installers.Core
{
    public class GameplayServicesInstaller : MonoInstaller
    {
        [SerializeField] private AdsManager _madPixelAdsManager;
        public override void InstallBindings()
        {
            BindUpgradesAvailabilityChecker();
            BindPauseManager();
            BindBattleSpeedManager();
            BindBeginGameManager();
            BindUpgradesCalculator();
            BindUpgradesServices();
            BindEvolutionService();
            BindTimerService();
            BindMadPixelAdsManager();
            BindAdsService();
            BindFoodBoostService();
            BindSpeedBoostService();
            BindBattleSpeedService();
            BindAnalyticsService();
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
        }
        private void BindMadPixelAdsManager() =>
            Container
                .BindInterfacesTo<AdsManager>()
                .FromInstance(_madPixelAdsManager)
                .AsSingle()
                .NonLazy();

        private void BindEvolutionService() =>
            Container
                .BindInterfacesAndSelfTo<EvolutionPresenter>()
                .AsSingle();

#if cas_advertisment_enabled
        private void BindAdsService() =>
            Container
                .BindInterfacesAndSelfTo<CasAdsService>()
                .AsSingle();
#endif
        private void BindAdsService() =>
            Container.BindInterfacesAndSelfTo<MaxAdsService>()
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