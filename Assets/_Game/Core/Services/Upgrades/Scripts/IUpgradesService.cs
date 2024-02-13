using System;
using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public interface IUpgradesService 
    {
        event Action<List<UpgradeUnitItemModel>> UpgradeUnitItemsUpdated;
        List<UpgradeUnitItemModel> GetUpgradeUnitItems();
        void PurchaseUnit(UnitType type);
        float GetFoodProductionSpeed();
        int GetInitialFoodAmount();
        UniTask Init();
    }
}