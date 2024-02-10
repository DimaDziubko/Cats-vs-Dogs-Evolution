using _Game.Core.GameState;
using Zenject;

namespace _Game.Core.Scripts
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BootstrapState>().AsSingle().NonLazy();
            Container.Bind<LoadProgressState>().AsSingle().NonLazy();
            Container.Bind<MenuState>().AsSingle().NonLazy();
            Container.Bind<PrepareBattleState>().AsSingle().NonLazy();
            Container.Bind<GameLoopState>().AsSingle().NonLazy();

            Container
                .BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();
        }
    }
}