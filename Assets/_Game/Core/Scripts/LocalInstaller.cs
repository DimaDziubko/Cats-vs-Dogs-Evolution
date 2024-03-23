using _Game.Common;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.GameModes.BattleMode.Scripts;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.CoinCounter.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI.GameplayUI.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class LocalInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private BaseFactory _baseFactory;
        [SerializeField] private ProjectileFactory _projectileFactory;
        [SerializeField] private CoinFactory _coinFactory;
        [SerializeField] private VfxFactory _vfxFactory;
        
        [SerializeField] private GameplayUI _gameplayUI;
        
        [SerializeField] private BattleField _battleField;
        [SerializeField] private BattleMode _battleMode;
        
        public override void InstallBindings()
        {
            BindFactories();

            BindGameplayUI();
            BindFoodGenerator();
            BindUnitBuilderViewController();

            BindBattleField();
            BindBattleMode();

            BindCoinCounter();
            
            BindRewardAnimator();

            BindBaseDestructionManager();
        }

        private void BindBaseDestructionManager()
        {
            Container.BindInterfacesAndSelfTo<BaseDestructionManager>().AsSingle();
        }


        private void BindRewardAnimator() =>
            Container.BindInterfacesAndSelfTo<RewardAnimator>().AsSingle();

        private void BindCoinCounter() => 
            Container.BindInterfacesAndSelfTo<CoinCounter>().AsSingle();

        private void BindBattleMode() =>
            Container.Bind<BattleMode>().FromInstance(_battleMode).AsSingle();

        private void BindBattleField() =>
            Container.Bind<BattleField>().FromInstance(_battleField).AsSingle();

        private void BindGameplayUI()
        {
            var cameraService = Container.Resolve<IWorldCameraService>();
            _gameplayUI.Construct(cameraService);
            Container.Bind<GameplayUI>().FromInstance(_gameplayUI).AsSingle();
        }

        private void BindFoodGenerator() => 
            Container.Bind<IFoodGenerator>().To<FoodGenerator>().AsSingle();

        private void BindUnitBuilderViewController() => 
            Container.Bind<IUnitBuilder>().To<UnitBuilderViewController>().AsSingle();
        

        private void BindFactories()
        {
            var battleState = Container.Resolve<IBattleStateService>();
            var ageState = Container.Resolve<IAgeStateService>();
            var cameraService = Container.Resolve<IWorldCameraService>();
            var random = Container.Resolve<IRandomService>();
            var audioService = Container.Resolve<IAudioService>();

            BindProjectileFactory(battleState, ageState, audioService);
            BindUnitFactory(battleState, ageState, cameraService, random, audioService);
            BindBaseFactory(battleState, ageState, cameraService);
            BindCoinFactory();
            BindVfxFactory(battleState, ageState);
        }

        private void BindCoinFactory() => 
            Container.Bind<ICoinFactory>().To<CoinFactory>().FromInstance(_coinFactory).AsSingle();

        private void BindVfxFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState)
        {
            _vfxFactory.Initialize(battleState, ageState);
            Container.Bind<IVfxFactory>().To<VfxFactory>().FromInstance(_vfxFactory).AsSingle();
        }

        private void BindProjectileFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IAudioService audioService)
        {
            _projectileFactory.Initialize(battleState, ageState, audioService);
            Container.Bind<IProjectileFactory>().To<ProjectileFactory>().FromInstance(_projectileFactory).AsSingle();
        }

        private void BindUnitFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IWorldCameraService cameraService,
            IRandomService random,
            IAudioService audioService)
        {
            _unitFactory.Initialize(battleState, ageState, cameraService, random, audioService);
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }

        private void BindBaseFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IWorldCameraService cameraService)
        {
            _baseFactory.Initialize(battleState, ageState, cameraService);
            Container.Bind<IBaseFactory>().To<BaseFactory>().FromInstance(_baseFactory).AsSingle();
        }
    }
}
