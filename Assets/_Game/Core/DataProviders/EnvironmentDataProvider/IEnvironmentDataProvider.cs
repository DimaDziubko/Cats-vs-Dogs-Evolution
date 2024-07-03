using Assets._Game.UI._Environment;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.EnvironmentDataProvider
{
    public interface IEnvironmentDataProvider
    {
        UniTask<EnvironmentData> Load(string key, int cacheContext);
    }
}