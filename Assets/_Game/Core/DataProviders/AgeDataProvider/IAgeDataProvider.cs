using _Game.Core.Data.Age.Static;
using Assets._Game.Core.Data.Age.Static;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.AgeDataProvider
{
    public interface IAgeDataProvider
    {
        UniTask<AgeStaticData> Load();
    }
}