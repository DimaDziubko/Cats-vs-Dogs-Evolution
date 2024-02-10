using _Game.Core.Services.Battle;
using _Game.GameModes.BattleMode.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;
using UnityEngine;

namespace _Game.Core.GameState
{
    public class MenuState : IState
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly IBattleStateService _battleState;

        private Disposable<MainMenu> _mainMenu;

        private BattleMode _battleMode;

        public MenuState(
            IMainMenuProvider mainMenuProvider,
            IBattleStateService battleState)
        {
            _mainMenuProvider = mainMenuProvider;
            _battleState = battleState;
            
        }

        public async void Enter()
        {
            //TODO Delete
            Debug.Log("ENTER Menu State");
            
            _mainMenu = await _mainMenuProvider.Load();
        }
        
        public void Exit()
        {
            _mainMenu.Dispose();
        }
        
    }
}
