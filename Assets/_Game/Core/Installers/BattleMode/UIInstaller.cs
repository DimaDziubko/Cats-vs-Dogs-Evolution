using Assets._Game.Common;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.UI._BattleUIController;
using Assets._Game.UI._Environment;
using Assets._Game.UI._Hud;
using Assets._Game.UI.UnitBuilderBtn.Scripts;
using UnityEngine;
using Zenject;

namespace Assets._Game.Core.Installers.BattleMode
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