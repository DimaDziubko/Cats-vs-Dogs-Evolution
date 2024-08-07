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
        }

        private void BindRaceSelectionController() =>
            Container.BindInterfacesAndSelfTo<RaceSelectionController>().AsSingle().NonLazy();

        private void BindDailyTaskGenerator() => 
            Container.BindInterfacesAndSelfTo<DailyTaskGenerator>().AsSingle().NonLazy();
    }
}