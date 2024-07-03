using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.UnitDataProviders
{
    public interface IUnitDataProvider
    {
        UniTask<UnitData> LoadUnitData(UnitLoadOptions options);
    }
}