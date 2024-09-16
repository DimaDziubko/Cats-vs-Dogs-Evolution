using _Game.Core.DataProviders.Facade;
using _Game.UI._Environment;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.EnvironmentDataLoader
{
    public interface IEnvironmentDataLoader
    {
        UniTask<EnvironmentData> Load(string key, LoadContext context);
    }
}