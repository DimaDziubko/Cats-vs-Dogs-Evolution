using _Game.Core.Data.Age.Static;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.AgeDataProvider
{
    public interface IAgeDataProvider
    {
        UniTask<AgeStaticData> Load(int timelineId);
    }
}