using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Providers;
using Zenject;

namespace _Game.Core.Scripts
{
    public class ConfigServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigProviders();
            BindConfigControllers();
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

        private void BindConfigControllers() =>
            Container
                .BindInterfacesAndSelfTo<GameConfigController>()
                .AsSingle();
    }
}