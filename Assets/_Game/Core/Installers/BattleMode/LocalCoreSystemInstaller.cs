using _Game.Gameplay._Race;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class LocalCoreSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindRaceSelectionController();
        }

        private void BindRaceSelectionController() =>
            Container.BindInterfacesAndSelfTo<RaceSelectionController>().AsSingle().NonLazy();
        
    }
}