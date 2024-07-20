using _Game.UI.RateGame.Scripts;
using Assets._Game.Core._SystemUpdate;
using Assets._Game.Gameplay._Race;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class LocalCoreSystemInstaller : MonoInstaller
    {
        [SerializeField] private SystemUpdate _systemUpdate;

        public override void InstallBindings()
        {
            BindSystemUpdate();
            BindRaceSelectionController();
            BindRateGameChecker();

            Debug.Log("RateGame BINDS");
        }

        private void BindRateGameChecker() =>
            Container.Bind<IRateGameChecker>().To<RateGameChecker>()
            .AsSingle()
            .NonLazy();

        private void BindRaceSelectionController() =>
            Container.Bind<RaceSelectionController>().AsSingle();

        private void BindSystemUpdate() =>
            Container.Bind<ISystemUpdate>().To<SystemUpdate>().FromInstance(_systemUpdate).AsSingle();
    }
}