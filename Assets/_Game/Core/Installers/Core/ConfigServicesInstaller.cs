using _Game.Core.Configs.Providers;
using _Game.Core.Configs.Repositories;
using Assets._Game.Core.Configs.Repositories;
using Zenject;

namespace Assets._Game.Core.Installers.Core
{
    public class ConfigServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigProviders();
            BindCommonConfigRepository();
            BindTimelineConfigRepository();
            BindAgeConfigRepository();
            BindBattleConfigRepository();
            BindEconomyConfigRepository();
        }

        private void BindConfigProviders()
        {
            Container
                .BindInterfacesAndSelfTo<RemoteConfigProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<LocalConfigProvider>()
                .AsSingle();
        }

        private void BindCommonConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<CommonItemsConfigRepository>()
                .AsSingle();

        
        private void BindEconomyConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<EconomyConfigRepository>()
                .AsSingle();

        private void BindTimelineConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<TimelineConfigRepository>()
                .AsSingle();

        private void BindAgeConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<AgeConfigRepository>()
                .AsSingle();

        private void BindBattleConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<BattleSpeedConfigRepository>()
                .AsSingle();
    }
}