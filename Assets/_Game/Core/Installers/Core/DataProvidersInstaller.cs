using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.DataProviders.Common;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataProvidersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitDataProvider();
            BindCommonItemsDataProvider();
        }

        private void BindUnitDataProvider()
        {
            Container.BindInterfacesAndSelfTo<UnitDataProvider>().AsSingle();
        }
        
        private void BindCommonItemsDataProvider() =>
            Container.BindInterfacesAndSelfTo<CommonItemsDataProvider>()
                .AsSingle();

    }
}