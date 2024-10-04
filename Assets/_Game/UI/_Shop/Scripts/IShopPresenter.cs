namespace _Game.UI._Shop.Scripts
{
    public interface IShopPresenter
    {
        Shop Shop { set; }
        void OnShopOpened();
        void OnShopClosed();
    }
}