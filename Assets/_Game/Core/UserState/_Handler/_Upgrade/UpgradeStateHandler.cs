using System;
using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

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
        }
    }
}