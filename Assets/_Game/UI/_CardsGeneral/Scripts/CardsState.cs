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

        private Disposable<CardsScreen> _cardsScreen;
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
            _cardsScreen = await _provider.Load();

            if (_cardsScreen?.Value != null)
            {
                _cardsScreen.Value.Show();
                _uiNotifier.OnScreenOpened(GameScreen.Cards);
            }
        }

        public void Exit()
        {
            if (_cardsScreen?.Value != null)
            {
                _cardsScreen.Value.Hide();
                _cardsScreen.Dispose();
                _cardsScreen = null;
            }
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.Cards);
        }

        public void Cleanup()
        {
            if (_cardsScreen?.Value != null)
            {
                _cardsScreen.Value.Hide();
                _cardsScreen.Dispose();
                _cardsScreen = null;
            }
            _button.UnHighlightBtn();
        }
    }
}