using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Gameplay._Bases.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.BaseDataLoader
{
    public interface IBaseStaticDataLoader
    {
        UniTask<BaseStaticData> Load(BaseLoadOptions options);
    }
}