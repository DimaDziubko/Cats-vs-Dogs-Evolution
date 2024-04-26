using _Game.Common;
using _Game.Core.Services.Camera;
using _Game.Gameplay.GameResult.Scripts;
using _Game.UI._BattleUIController;
using _Game.UI._Environment;
using _Game.UI._Hud;
using _Game.UI.GameplayUI.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private Hud _hud;
        [SerializeField] private GameplayUI _gameplayUI;

        public override void InstallBindings()
        {
            BindHud();
            BindGameplayUI();
            BindRewardAnimator();
            BindBattleUIController();
            BindGameResultHandler();
            BindEnvironmentController();
        }

        private void BindGameResultHandler() => 
            Container.Bind<GameResultHandler>().AsSingle();

        private void BindHud() => 
            Container.Bind<Hud>().FromInstance(_hud).AsSingle();

        private void BindBattleUIController() => 
            Container.Bind<BattleUIController>().AsSingle();
        
        private void BindRewardAnimator() =>
            Container.BindInterfacesAndSelfTo<RewardAnimator>().AsSingle();

        private void BindGameplayUI()
        {
            var cameraService = Container.Resolve<IWorldCameraService>();
            _gameplayUI.Construct(cameraService);
            Container.Bind<GameplayUI>().FromInstance(_gameplayUI).AsSingle();
        }

        private void BindEnvironmentController() => 
            Container.Bind<EnvironmentController>().AsSingle();
    }
}