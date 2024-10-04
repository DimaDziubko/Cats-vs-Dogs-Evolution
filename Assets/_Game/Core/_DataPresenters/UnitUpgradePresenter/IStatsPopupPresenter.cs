using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataPresenters.UnitUpgradePresenter
{
    public interface IStatsPopupPresenter
    {
        UnitType FindNextAvailableModel(UnitType type, bool forward, out bool isAvailable);
        StatsPopupModel GetStatsPopupModelFor(UnitType type);
    }
}