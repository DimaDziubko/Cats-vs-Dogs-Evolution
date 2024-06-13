using _Game.Gameplay._Bases.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.BaseDataProvider
{
    public interface IBaseStaticDataProvider
    {
        UniTask<BaseStaticData> Load(BaseLoadOptions options);
    }
}