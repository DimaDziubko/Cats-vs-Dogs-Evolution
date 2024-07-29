using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public interface IUpgradeStateHandler
    {
        void UpgradeItem(UpgradeItemType type, float price);
    }
}