using _Game.Gameplay._Cards.Scripts;
using _Game.Gameplay._DailyTasks.Scripts;
using _Game.Gameplay._Race;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class LocalCoreSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindRaceSelectionController();
            BindDailyTaskGenerator();
            BindDailyTaskCompletionChecker();
            BindDailyTaskPresenter();
            BindCardGenerator();
        }

        private void BindCardGenerator() =>
            Container     
                .BindInterfacesAndSelfTo<CardGenerator>()
                .AsSingle()
                .NonLazy();

        private void BindRaceSelectionController() =>
            Container
                .BindInterfacesAndSelfTo<RaceSelectionController>()
                .AsSingle()
                .NonLazy();

        private void BindDailyTaskGenerator() =>
            Container
                .BindInterfacesAndSelfTo<DailyTaskGenerator>()
                .AsSingle().NonLazy();

        private void BindDailyTaskCompletionChecker() =>
            Container
                .BindInterfacesAndSelfTo<DailyTaskCompletionChecker>()
                .AsSingle()
                .NonLazy();

        private void BindDailyTaskPresenter() =>
            Container
                .BindInterfacesAndSelfTo<DailyTaskPresenter>()
                .AsSingle()
                .NonLazy();
    }
}