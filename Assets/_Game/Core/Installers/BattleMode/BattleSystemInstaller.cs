using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleStateHandler;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
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
            BindEndBattleHandler();
            BindBattleMode();
        }
        
        private void BindFoodGenerator() =>
            Container.BindInterfacesAndSelfTo<FoodGenerator>().AsSingle();
        
        private void BindCoinCounter() =>
            Container.BindInterfacesAndSelfTo<CoinCounter>().AsSingle();

        private void BindBaseDestructionManager() => 
            Container.BindInterfacesAndSelfTo<BaseDestructionManager>().AsSingle();

        private void BindBattleField() =>
            Container.Bind<BattleField>().AsSingle();
        
        private void BindUnitBuilderViewController() =>
            Container.BindInterfacesAndSelfTo<UnitBuilderViewController>().AsSingle();

        private void BindBattle() =>
            Container.BindInterfacesAndSelfTo<Battle>().AsSingle().NonLazy();

        private void BindBattleStateHandler() => 
            Container.BindInterfacesAndSelfTo<BattleStateHandler>().AsSingle();

        private void BindBattleMode() =>
            Container.BindInterfacesAndSelfTo<_BattleModes.Scripts.BattleMode>().AsSingle().NonLazy();

        private void BindEndBattleHandler() => 
            Container.BindInterfacesAndSelfTo<EndBattleHandler>().AsSingle();
    }
    
}