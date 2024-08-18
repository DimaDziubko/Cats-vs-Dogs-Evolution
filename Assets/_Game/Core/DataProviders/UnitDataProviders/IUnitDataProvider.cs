using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.UnitDataProviders
{
    public interface IUnitDataProvider
    {
        UnitData LoadUnitData(UnitLoadOptions options);
    }
}