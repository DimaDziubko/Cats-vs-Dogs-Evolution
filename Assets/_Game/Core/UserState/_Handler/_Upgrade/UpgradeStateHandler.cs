using System;
using System.Collections.Generic;
using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public class UpgradeStateHandler : IUpgradeStateHandler
    {
        private readonly IUserContainer _userContainer;

        public UpgradeStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void UpgradeItem(UpgradeItemType type, float price)
        {
            if (_userContainer.State.Currencies.Coins >= price)
            {
                _userContainer.State.Currencies.ChangeCoins(price, false, CurrenciesSource.Upgrade);
            
                switch (type)
                {
                    case UpgradeItemType.FoodProduction:
                        _userContainer.State.TimelineState.ChangeFoodProductionLevel();
                        break;
                    case UpgradeItemType.BaseHealth:
                        _userContainer.State.TimelineState.ChangeBaseHealthLevel();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
            
            _userContainer.RequestSaveGame(true);

        }
        
        public void ChangeCardSummoningLevel(int newLevel)
        {
            _userContainer.State.CardsCollectionState.ChangeCardSummoningLevel(newLevel);
            _userContainer.RequestSaveGame();

        }

        public void AddCards(List<int> cardsId)
        {
            _userContainer.State.CardsCollectionState.AddCards(cardsId);
            _userContainer.RequestSaveGame();
        }

        public void UpdateLastDropIdx(int nextIndex)
        {
            _userContainer.State.CardsCollectionState.ChangeLastDropIdx(nextIndex);
        }

        public void UpgradeCard(int id, int needForUpgrade)
        {
            _userContainer.State.CardsCollectionState.UpgradeCard(id, needForUpgrade);
            _userContainer.RequestSaveGame();

        }
    }
}