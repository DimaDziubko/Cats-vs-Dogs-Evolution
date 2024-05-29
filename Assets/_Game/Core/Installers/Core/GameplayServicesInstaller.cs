using _Game.Core.Ads;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Timer.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI.Pin.Scripts;
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
            BindBattleState();
            BindUpgradesServices();
            BindEvolutionService();
            BindAgeStateService();
            BindAdsService();
            BindFoodBoostService();
            BindTimerService();
            BindBattleSpeedService();
            BindAnalyticsService();
            BindDTDAnalyticsService();
        }

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

        private void BindBattleState() =>
            Container
                .BindInterfacesAndSelfTo<BattleStateService>()
                .AsSingle();

        private void BindUpgradesServices()
        {
            Container
                .BindInterfacesAndSelfTo<EconomyUpgradesService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<UnitUpgradesService>()
                .AsSingle();
        }

        private void BindEvolutionService() =>
            Container
                .BindInterfacesAndSelfTo<EvolutionService>()
                .AsSingle();

        private void BindAgeStateService() =>
            Container
                .BindInterfacesAndSelfTo<AgeStateService>()
                .AsSingle();

        private void BindAdsService() =>
            Container
                .Bind<IAdsService>().To<CasAdsService>()
                .AsSingle();

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
    }
}