﻿using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class BattleState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IStartBattleScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;
        
        private readonly ToggleButton _button;

        public BattleState(
            MainMenu mainMenu,
            IStartBattleScreenProvider provider,
            ToggleButton button,
            IUINotifier uiNotifier)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _button = button;
            _uiNotifier = uiNotifier;
        }
        
        public async void Enter()
        {
            _mainMenu.SetActiveButton(_button);
            _mainMenu.HideCurtain();
            _button.HighlightBtn();
            _mainMenu.RebuildLayout();
            
            var startBattleWindow = await _provider.Load();
            startBattleWindow.Value.Show();
            _uiNotifier.OnScreenOpened(GameScreen.Battle);
        }

        public void Exit()
        {
            _mainMenu.ShowCurtain();

            _provider.Unload();
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.Battle);
        }

        public void Cleanup()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
        }
    }
}