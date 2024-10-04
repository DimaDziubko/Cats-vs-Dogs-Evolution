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

            var shop = await _provider.Load();
            shop.Value.Show();
            _uiNotifier.OnScreenOpened(GameScreen.Shop);
            
        }

        public void Exit()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.Shop);
        }

        public void Cleanup()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
        }
    }
}