using _Game.UI._Shop._MiniShop.Scripts;

namespace _Game.UI._Shop.Scripts
{
    public interface IMiniShopPresenter
    {
        MiniShop MiniShop { set; }
        void OnMiniShopOpened();
        void OnMiniShopClosed();
    }
}