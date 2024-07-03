using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Core.DataPresenters.UnitDataPresenter
{
    public interface IUnitDataPresenter
    {
        UnitData GetUnitData(UnitType type, int context);
    }
}