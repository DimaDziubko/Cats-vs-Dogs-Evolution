using System;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;

namespace _Game.Core.GameState
{
    public class MenuState : IState
    {
        private readonly IMainMenuProvider _mainMenuProvider;

        private Disposable<MainMenu> _mainMenu;
        
        public MenuState(IMainMenuProvider mainMenuProvider) =>
            _mainMenuProvider = mainMenuProvider;

        public async void Enter()
        {
            _mainMenu = await _mainMenuProvider.Load();
            _mainMenu.Value.Show();
        }

        public void Exit()
        {
            _mainMenu.Value.Hide();
            _mainMenu.Dispose();
        }
    }
}
