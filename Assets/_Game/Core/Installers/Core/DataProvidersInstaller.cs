using _Game.Core.DataProviders;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataProvidersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitDataProvider();
            BindWeaponDataProvider();
            BaseDataProvider();
        }

        private void BindUnitDataProvider() => 
            Container.BindInterfacesAndSelfTo<UnitDataProvider>()
                .AsSingle();

        private void BindWeaponDataProvider() => 
            Container.BindInterfacesAndSelfTo<WeaponDataProvider>()
                .AsSingle();

        private void BaseDataProvider() => 
            Container.BindInterfacesAndSelfTo<BaseDataProvider>()
                .AsSingle();
    }
}