using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class ShopState : IMenuState
    {
        private readonly MainMenu _mainMenu;
        private readonly IShopProvider _provider;

        private Disposable<Shop> _shop;
        private readonly ToggleButton _button;

        public ShopState(
            MainMenu mainMenu,
            IShopProvider provider,
            ToggleButton button)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _button = button;
        }
        
        public async void Enter()
        {
            _mainMenu.SetActiveButton(_button);
            _button.HighlightBtn();
            _mainMenu.RebuildLayout();
            _shop = await _provider.Load();
            _shop.Value.Show();
            //_shop.Value.Init();
        }

        public void Exit()
        {
            _shop.Value.Hide();
            _shop.Dispose();
            _shop = null;
            _button.UnHighlightBtn();
        }

        public void Cleanup()
        {
            if (_shop != null)
            {
                _shop.Value.Hide();
                _shop.Dispose();
                _shop = null;
                _button.UnHighlightBtn();
            }
        }
    }
}