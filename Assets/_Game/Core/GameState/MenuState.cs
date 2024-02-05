using _Game.GameModes.BattleMode.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;
using UnityEngine;
using Zenject;

namespace _Game.Core.GameState
{
    public class MenuState : IState
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        
        private Disposable<MainMenu> _mainMenu;
        
        private BattleMode _battleMode;

        [Inject]
        public MenuState(IMainMenuProvider mainMenuProvider)
        {
            _mainMenuProvider = mainMenuProvider;
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
