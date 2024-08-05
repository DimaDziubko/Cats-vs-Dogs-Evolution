using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.Utils;
using Assets._Game.Core.Factory;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Zenject;

namespace _Game._BattleModes.Scripts
{
    public class BattleMode : 
        IInitializable,
        IGameModeCleaner,
        IBaseDestructionHandler
    {
        public IEnumerable<GameObjectFactory> Factories { get; private set; }
        public string SceneName => Constants.Scenes.BATTLE_MODE;
        
        private readonly IBaseDestructionManager _baseDestructionManager;

        private readonly IBattleManager _battleManager;
        private readonly Battle _battle;

        public BattleMode(
            IBaseDestructionManager baseDestructionManager,
            IFactoriesHolder factoriesHolder,
            IBattleManager battleManager,
            Battle battle)
        {
            Factories = factoriesHolder.Factories;

            _baseDestructionManager = baseDestructionManager;
            _battleManager = battleManager;
            _battle = battle;
        }
        
        void  IInitializable.Initialize() => _baseDestructionManager.Register(this);

        public void ResetGame() => 
            _battle.Reset();
        
        void IGameModeCleaner.Cleanup()
        {
            _battle.Cleanup();
        }

        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base) => 
            _battleManager.StopBattle();

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            switch (faction)
            {
                case Faction.Player:
                    _battleManager.EndBattle(GameResultType.Defeat);
                    break;
                case Faction.Enemy:
                    _battleManager.EndBattle(GameResultType.Victory);
                    break;
            }
        }
        
    }
}