using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class CardsState : ILocalState
    {
        private readonly GeneralCardsScreen _generalCardsScreen;
        private readonly ICardsScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;
        
        private readonly ToggleButton _button;

        public CardsState(
            GeneralCardsScreen generalCardsScreen,
            ICardsScreenProvider provider,
            ToggleButton button,
            IUINotifier uiNotifier)
        {

            _generalCardsScreen = generalCardsScreen;
            _provider = provider;
            _button = button;
            _uiNotifier = uiNotifier;
        }
        
        public async void Enter()
        {
            _generalCardsScreen.SetActiveButton(_button);
            _button.HighlightBtn();
            
            var cardsScreen = await _provider.Load();

            cardsScreen.Value.Show();
            _uiNotifier.OnScreenOpened(GameScreen.Cards);
        }

        public void Exit()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.Cards);
        }

        public void Cleanup()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
        }
    }
}