using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public interface IMiniShopProvider
    {
        UniTask<Disposable<MiniShop>> Load();
    }
}