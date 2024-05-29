using _Game.Core.Configs.Providers;
using _Game.Core.Configs.Repositories;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class ConfigServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigProviders();
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

        private void BindEconomyConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<EconomyConfigRepository>()
                .AsSingle();

        private void BindTimelineConfigRepository()
        {
            Container
                .BindInterfacesAndSelfTo<TimelineConfigRepository>()
                .AsSingle();
        }

        private void BindAgeConfigRepository()
        {
            Container
                .BindInterfacesAndSelfTo<AgeConfigRepository>()
                .AsSingle();
        }

        private void BindBattleConfigRepository()
        {
            Container
                .BindInterfacesAndSelfTo<BattleSpeedConfigRepository>()
                .AsSingle();
        }
    }
}