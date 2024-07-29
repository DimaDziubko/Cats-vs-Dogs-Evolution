using System;
using System.Collections.Generic;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace Assets._Game.Core.DataPresenters.UnitUpgradePresenter
{
    public interface IUnitUpgradesPresenter
    {
        event Action<Dictionary<UnitType, UnitUpgradeItemModel>> UpgradeUnitItemsUpdated;
        void PurchaseUnit(UnitType type, float price);
        void OnUpgradesWindowOpened();
    }
}