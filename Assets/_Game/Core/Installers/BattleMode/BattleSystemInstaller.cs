using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleStateHandler;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.Food.Scripts;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class BattleSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFoodGenerator();
            BindCoinCounter();
            BindBaseDestructionManager();
            BindBattleField();
            BindUnitBuilderViewController();
            BindBattle();
            BindBattleStateHandler();
            BindBattleMode();
            BindBattleMediator();
        }

        private void BindFoodGenerator() =>
            Container.Bind<IFoodGenerator>().To<FoodGenerator>().AsSingle();

        private void BindCoinCounter() =>
            Container.BindInterfacesAndSelfTo<CoinCounter>().AsSingle();

        private void BindBaseDestructionManager() => 
            Container.BindInterfacesAndSelfTo<BaseDestructionManager>().AsSingle();

        private void BindBattleField() =>
            Container.Bind<BattleField>().AsSingle();

        private void BindUnitBuilderViewController() =>
            Container.Bind<IUnitBuilder>().To<UnitBuilderViewController>().AsSingle();

        private void BindBattle() =>
            Container.Bind<Battle>().AsSingle();

        private void BindBattleStateHandler() => 
            Container.Bind<BattleStateHandler>().AsSingle();

        private void BindBattleMode() =>
            Container.BindInterfacesAndSelfTo<GameModes._BattleMode.Scripts.BattleMode>().AsSingle();

        private void BindBattleMediator() => 
            Container.BindInterfacesAndSelfTo<BattleMediator>().AsSingle().NonLazy();
    }
    
}