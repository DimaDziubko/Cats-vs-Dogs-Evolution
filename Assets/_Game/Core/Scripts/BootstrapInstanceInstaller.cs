using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class BootstrapInstanceInstaller : MonoInstaller
    {
        [SerializeField] private GameBootstrapper _bootstrapper;
    
        public override void InstallBindings()
        {
            BindGameBootstrapper();
        }
    
        private void BindGameBootstrapper()
        {
            var bootstrapper = Container
                .InstantiatePrefabForComponent<GameBootstrapper>(_bootstrapper);
            
            Container.Bind<GameBootstrapper>().
                FromInstance(bootstrapper).AsSingle();
        }
    }
}
