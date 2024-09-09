using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataPresenters.UnitUpgradePresenter
{
    public interface IUnitUpgradesPresenter
    {
        event Action<Dictionary<UnitType, UnitUpgradeItemModel>> UpgradeUnitItemsUpdated;
        void PurchaseUnit(UnitType type, float price);
        void OnUpgradesWindowOpened();
        void ShowInfoFor(UnitType type);
    }
}