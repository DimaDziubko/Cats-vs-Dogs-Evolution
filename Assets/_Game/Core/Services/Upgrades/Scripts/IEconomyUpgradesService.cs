using System;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public interface IEconomyUpgradesService 
    {
        event Action<UpgradeItemViewModel> UpgradeItemUpdated;
        UniTask Init();
        float GetFoodProductionSpeed();
        int GetInitialFoodAmount();
        float GetBaseHealth();
        void UpgradeItem(UpgradeItemType type);
        void OnUpgradesWindowOpened();
    }
}