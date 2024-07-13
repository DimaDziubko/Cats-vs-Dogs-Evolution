using _Game.Core.DataProviders.Facade;
using Assets._Game.UI._Environment;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.EnvironmentDataProvider
{
    public interface IEnvironmentDataProvider
    {
        UniTask<EnvironmentData> Load(string key, LoadContext context);
    }
}