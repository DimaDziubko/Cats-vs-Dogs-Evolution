using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public interface IUnitDataProvider
    {
        IUnitData GetDecoratedUnitData(UnitType type, int context);
    }
}