using System;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace Assets._Game.Core.Data.Age.Dynamic._UpgradeItem
{
    public interface IUpgradeItemsReadonly
    {
        public event Action<UpgradeItemType,  UpgradeItemDynamicData> Changed;
        UpgradeItemDynamicData GetItemData(UpgradeItemType type);
    }
}