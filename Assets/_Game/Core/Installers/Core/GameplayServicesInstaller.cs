using _Game.Core.Ads;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay.BattleLauncher;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class GameplayServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindPauseManager();
            BindBeginGameManager();
            BindBattleState();
            BindUpgradesServices();
            BindEvolutionService();
            BindAgeStateService();
            BindAdsService();
            BindBonusRewardService();
        }

        private void BindPauseManager() =>
            Container.Bind<IPauseManager>()
                .To<PauseManager>()
                .AsSingle();

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
                .BindInterfacesAndSelfTo<AdsService>()
                .AsSingle();

        private void BindBonusRewardService() => 
            Container
                .BindInterfacesAndSelfTo<BonusRewardService>()
                .AsSingle();
    }
}