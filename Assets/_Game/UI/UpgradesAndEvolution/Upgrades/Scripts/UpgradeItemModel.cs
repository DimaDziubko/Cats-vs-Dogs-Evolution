using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Data.Age.Static._UpgradeItem;
using _Game.UI.Common.Scripts;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeItemModel
    {
        public UpgradeItemStaticData StaticData;
        public UpgradeItemDynamicData DynamicData;
        public string AmountText;
        public ButtonState ButtonState;
    }
}