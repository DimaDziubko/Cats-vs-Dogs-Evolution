using _Game.Core._Facebook;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class SDKServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFacebookInstaller();
        }

        private void BindFacebookInstaller()
        {
            Container.BindInterfacesAndSelfTo<FacebookInitializer>().AsSingle().NonLazy();
        }
    }
}