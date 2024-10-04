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

            BindCConfigRepositoryFacade();
        }

        private void BindCConfigRepositoryFacade() =>
            Container
                .BindInterfacesAndSelfTo<ConfigRepositoryFacade>()
                .AsSingle();
        
        private void BindConfigProviders()
        {
            Container
                .BindInterfacesAndSelfTo<RemoteConfigProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<LocalConfigProvider>()
                .AsSingle();
        }
        
    }
}