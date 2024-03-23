using _Game.Core.GameState;
using Zenject;

namespace _Game.Core.Scripts
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BootstrapState>().AsSingle().NonLazy();
            Container.Bind<ConfigurationState>().AsSingle().NonLazy();
            Container.Bind<LoginState>().AsSingle().NonLazy();
            Container.Bind<InitializationState>().AsSingle().NonLazy();
            Container.Bind<GameLoadingState>().AsSingle().NonLazy();
            Container.Bind<MenuState>().AsSingle().NonLazy();
            Container.Bind<GameLoopState>().AsSingle().NonLazy();

            Container
                .BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();
        }
    }
}