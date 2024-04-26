using _Game.Gameplay._Bases.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders
{
    public interface IBaseDataProvider
    {
        UniTask<BaseData> LoadBase(BaseLoadOptions options);
    }
}