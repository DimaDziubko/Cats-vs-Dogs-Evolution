using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core._DataPresenters.UnitUpgradePresenter
{
    public interface IUnitUpgradesPresenter
    {
        event Action<Dictionary<UnitType, UnitUpgradeItemModel>> UpgradeUnitItemsUpdated;
        void PurchaseUnit(UnitType type, float price);
        void OnUpgradesScreenOpened();
        void ShowInfoFor(UnitType type);
    }
}