using System.Collections.Generic;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public interface IUpgradeStateHandler
    {
        void UpgradeItem(UpgradeItemType type, float price);
        void ChangeCardSummoningLevel(int newLevel);
        void UpgradeCard(int id, int needForUpgrade);
        void AddCards(List<int> cardsId);
        void UpdateLastDropIdx(int nextIndex);
    }
}