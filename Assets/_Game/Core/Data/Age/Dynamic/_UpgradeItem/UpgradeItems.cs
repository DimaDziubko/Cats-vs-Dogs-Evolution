using System;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Data.Age.Dynamic._UpgradeItem
{
    public class UpgradeItems : IUpgradeItemsReadonly
    {
        public UpgradeItemDynamicData BaseHealth;
        public UpgradeItemDynamicData FoodProductionSpeed;

        public event Action<UpgradeItemType, UpgradeItemDynamicData> Changed;

        public void Change(UpgradeItemType type, UpgradeItemDynamicData value)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    FoodProductionSpeed = value;
                    break;
                case UpgradeItemType.BaseHealth:
                    BaseHealth = value;
                    break;
            }
            
            Changed?.Invoke(type, value);
        }

        public UpgradeItemDynamicData GetItemData(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return FoodProductionSpeed;
                case UpgradeItemType.BaseHealth:
                    return BaseHealth;
                default:
                    return new UpgradeItemDynamicData();
            }
        }
    }
}