using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Data.Age.Dynamic
{
    public class AgeDynamicData
    {
        public UpgradeItems UpgradeItems;
        public BoostsData BoostsData;

        public AgeDynamicData()
        {
            UpgradeItems = new UpgradeItems();
            BoostsData = new BoostsData()
            {
                CardsBaseHealthBoost = 1,
                CardsCoinsGainedBoost = 1,
                CardsFoodProductionBoost = 1,
                CardsAllUnitsDamageBoost = 1,
                CardsAllUnitsHealthBoost = 1
            };
        }
        
        public void ChangeUpgradeItemValue(UpgradeItemType type, UpgradeItemDynamicData newValue)
        {
            UpgradeItems.Change(type, newValue);
        }

        public void ChangeBoost(BoostSource source, BoostType boostType, float value) => 
            BoostsData.Change(source, boostType, value);
    }
}