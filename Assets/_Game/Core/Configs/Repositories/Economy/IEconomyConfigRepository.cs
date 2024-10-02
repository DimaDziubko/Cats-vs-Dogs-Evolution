using _Game.Core.Configs.Models;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Configs.Repositories.Economy
{
    public interface IEconomyConfigRepository
    {
        FoodBoostConfig GetFoodBoostConfig();
        UpgradeItemConfig GetConfigForType(UpgradeItemType type);
        int GetInitialFoodAmount();
    }
}