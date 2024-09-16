using _Game.Core.Data;
using _Game.UI._Shop.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.ShopDataProvider
{
    public interface IShopDataLoader
    {
        UniTask<DataPool<int, ShopItemStaticData>> LoadShopData();
    }
}