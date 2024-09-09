using _Game.Core.DataLoaders.UnitDataLoaders;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.UnitDataLoaders
{
    public interface IUnitDataLoader
    {
        UniTask<IUnitData> LoadUnitDataAsync(UnitLoadOptions options);
    }
}