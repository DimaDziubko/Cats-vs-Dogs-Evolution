﻿using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;

namespace _Game.UI._MainMenu.State
{
    public class GeneralCardsState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IGeneralCardsScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;
        
        private readonly ToggleButton _button;

        public GeneralCardsState(
            MainMenu mainMenu,
            IGeneralCardsScreenProvider provider,
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
            _button.HighlightBtn();
            _mainMenu.RebuildLayout();

            var generalCardsScreen = await _provider.Load();
            generalCardsScreen.Value.Show();
            _uiNotifier.OnScreenOpened(GameScreen.GeneralCards);
            
            _mainMenu.CardsTutorialStep.CancelStep();
        }

        public void Exit()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.GeneralCards);
            _mainMenu.ShowCardsTutorialWithDelay(0.5f);
        }

        public void Cleanup()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
        }
    }
}