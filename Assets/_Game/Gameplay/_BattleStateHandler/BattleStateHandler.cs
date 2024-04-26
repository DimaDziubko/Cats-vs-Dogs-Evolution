using _Game.Core.GameState;
using _Game.Gameplay.Battle.Scripts;

namespace _Game.Gameplay._BattleStateHandler
{
    public class BattleStateHandler
    {
        private readonly IGameStateMachine _stateMachine;
        
        private IBattleMediator _battleMediator;

        public BattleStateHandler(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }

        public void HandleStart()
        {
            _stateMachine.Enter<GameLoopState>();
        }

        public void GoToMainMenu()
        {
            _stateMachine.Enter<MenuState>();
        }
    }
}