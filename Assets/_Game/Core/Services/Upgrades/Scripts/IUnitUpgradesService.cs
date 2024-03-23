using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public interface IUnitUpgradesService
    {
        event Action<List<UpgradeUnitItemViewModel>> UpgradeUnitItemsUpdated;
        UniTask Init();
        void PurchaseUnit(UnitType type);
        void OnUpgradesWindowOpened();
    }
}