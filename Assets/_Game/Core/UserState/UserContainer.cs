using System;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core.UserState
{
    public class UserContainer : IPersistentDataService
    {
        public UserAccountState State { get; set; }
        
        public GameConfig GameConfig { get; set; }

        public void AddCoins(in float count)
        {
            State.Currencies.ChangeCoins(count, true);
        }

        public void PurchaseUnit(UnitType type, float price)
        {
            if (State.Currencies.Coins >= price)
            {
                ChangeAfterPurchase(price, false);
                State.TimelineState.OpenUnit(type);
            }
        }

        public void OpenNextAge()
        {
            State.TimelineState.OpenNextAge();
        }

        public void OpenNextTimeline()
        {
            State.TimelineState.OpenNextTimeline();
        }

        public void OpenNextBattle(int nextBattle)
        {
            State.TimelineState.OpenNextBattle(nextBattle);
        }

        public void SetAllBattlesWon(bool allBattlesWon)
        {
            State.TimelineState.SetAllBattlesWon(true);
        }

        public void UpgradeItem(UpgradeItemType type, float price)
        {
            //TODO Delete
            Debug.Log($"Upgrade item user container type: {type} price: {price}");
            
            if (State.Currencies.Coins >= price)
            {
                ChangeAfterPurchase(price, false);
                
                switch (type)
                {
                    case UpgradeItemType.FoodProduction:
                        State.TimelineState.ChangeFoodProductionLevel();
                        break;
                    case UpgradeItemType.BaseHealth:
                        State.TimelineState.ChangeBaseHealthLevel();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        private void ChangeAfterPurchase(float price, bool isPositive)
        {
            price = isPositive ? price : (price * -1);
            State.Currencies.ChangeCoins(price, isPositive);
        }
    }
}