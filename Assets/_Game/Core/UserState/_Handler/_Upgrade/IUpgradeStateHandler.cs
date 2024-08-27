using System.Collections.Generic;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public interface IUpgradeStateHandler
    {
        void UpgradeItem(UpgradeItemType type, float price);
        void ChangeCardSummoningLevel(int newLevel);
        void UpgradeCard(int id);
        void BuyCard(int amount, int price);
        void AddCards(List<int> cardsId);
    }
}