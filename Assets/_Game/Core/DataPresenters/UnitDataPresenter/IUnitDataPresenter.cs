using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataPresenters.UnitDataPresenter
{
    public interface IUnitDataPresenter
    {
        UnitData GetUnitData(UnitType type, int context);
    }
}