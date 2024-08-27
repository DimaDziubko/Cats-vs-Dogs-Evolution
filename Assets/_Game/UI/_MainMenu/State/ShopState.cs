using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class ShopState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IShopProvider _provider;
        private readonly IUINotifier _uiNotifier;

        private Disposable<Shop> _shop;
        private readonly ToggleButton _button;

        public ShopState(
            MainMenu mainMenu,
            IShopProvider provider,
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

            _shop = await _provider.Load();

            if (_shop?.Value != null)
            {
                _shop.Value.Show();
                //_shop.Value.Init(); // Uncomment if initialization is required
                _uiNotifier.OnScreenOpened(GameScreen.Shop);
            }
        }

        public void Exit()
        {
            if (_shop?.Value != null)
            {
                _shop.Value.Hide();
                _shop.Dispose();
                _shop = null;
            }
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.Shop);
        }

        public void Cleanup()
        {
            if (_shop != null)
            {
                if (_shop.Value != null)
                {
                    _shop.Value.Hide();
                }
                _shop.Dispose();
                _shop = null;
            }
            _button.UnHighlightBtn();
        }
    }
}