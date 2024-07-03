using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.Shop.Scripts
{
    public interface IShopPopupProvider 
    {
        UniTask<Disposable<ShopPopup>> Load();
    }
}