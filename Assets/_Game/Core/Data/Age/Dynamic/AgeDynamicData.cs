using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Data.Age.Dynamic
{
    public class AgeDynamicData
    {
        public UpgradeItems UpgradeItems;

        public AgeDynamicData()
        {
            UpgradeItems = new UpgradeItems();
        }
        
        public void ChangeUpgradeItemValue(UpgradeItemType type, UpgradeItemDynamicData newValue)
        {
            UpgradeItems.Change(type, newValue);
        }
    }
}