using _Game.Core.Data.Age.Static;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.AgeDataProvider
{
    public interface IAgeDataLoader
    {
        UniTask<AgeStaticData> Load(int timelineId);
    }
}