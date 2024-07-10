using System.Collections.Generic;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Battle.Scripts;
using Assets._Game.Core.Factory;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.BattleLauncher;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.Utils;

namespace Assets._Game.GameModes._BattleMode.Scripts
{
    public class BattleMode : IGameModeCleaner, IBattleLauncher, IBaseDestructionHandler
    {
        public IEnumerable<GameObjectFactory> Factories { get; private set; }
        public string SceneName => Constants.Scenes.BATTLE_MODE;
        
        private readonly IBaseDestructionManager _baseDestructionManager;
        private readonly IBattleLaunchManager _battleLaunchManager;
        
        private IBattleMediator _battleMediator;

        public BattleMode(
            IBattleLaunchManager battleLaunchManager,
            IBaseDestructionManager baseDestructionManager,
            IFactoriesHolder factoriesHolder)
        {
            Factories = factoriesHolder.Factories;

            _baseDestructionManager = baseDestructionManager;
            _battleLaunchManager = battleLaunchManager;
        }
        
        
        public void Init()
        {
            _battleLaunchManager.Register(this);
            _baseDestructionManager.Register(this);
        }

        public void ResetGame() => 
            _battleMediator.Reset();

        void IGameModeCleaner.Cleanup() => 
            _battleMediator.Cleanup();

        void IBattleLauncher.LaunchBattle() => 
            _battleMediator.StartBattle();

        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base) => 
            _battleMediator.StopBattle();

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            switch (faction)
            {
                case Faction.Player:
                    _battleMediator.EndBattle(GameResultType.Defeat);
                    break;
                case Faction.Enemy:
                    _battleMediator.EndBattle( GameResultType.Victory);
                    break;
            }
        }

        public void SetMediator(IBattleMediator battleMediator) => 
            _battleMediator = battleMediator;
    }
}