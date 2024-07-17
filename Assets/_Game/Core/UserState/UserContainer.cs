using System;
using _Game.Core.Configs.Models;
using _Game.Core.Debugger;
using _Game.Core.Services.UserContainer;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState
{
    public class UserContainer : IUserContainer
    {
        public UserAccountState State { get; set; }
        public GameConfig GameConfig { get; set; }

        public UserContainer(IMyDebugger debugger)
        {
            debugger.UserContainer = this;
        }
        
        public void AddAdsReviewed() => 
            State.AdsStatistics.AddAdsReviewed();

        public void FirstDayRetentionSent() => 
            State.RetentionState.ChangeFirstDayRetentionEventSent(true);
        public void SecondDayRetentionSent() => 
            State.RetentionState.ChangeSecondDayRetentionEventSent(true);

        public void AddCompletedBattle() => 
            State.BattleStatistics.AddCompletedBattle();

        public void CompleteTutorialStep(int step) => 
            State.TutorialState.ChangeCompletedStep(step);

        public void ChangeNormalSpeed(bool isNormal) => 
            State.BattleSpeedState.SetNormalSpeedActive(isNormal);

        public void ChangeBattleTimerDurationLeft(float timerTimeLeft) => 
            State.BattleSpeedState.ChangeDurationLeft(timerTimeLeft);

        public void AddCoins(in float count) => 
            State.Currencies.ChangeCoins(count, true);

        public void PurchaseUnit(UnitType type, float price)
        {
            if (State.Currencies.Coins >= price)
            {
                ChangeAfterPurchase(price, false);
                State.TimelineState.OpenUnit(type);
            }
        }

        public void OnOpenNextAge()
        {
            State.Currencies.RemoveAllCoins();
            State.TimelineState.OpenNextAge();
        }

        public void OpenNextTimeline()
        {
            State.Currencies.RemoveAllCoins();
            State.TimelineState.OpenNextTimeline();
        }

        public void RecoverFoodBoost(int amount, DateTime lastDailyFoodBoost) => 
            ChangeFoodBoost(amount, true, lastDailyFoodBoost);

        public void SpendFoodBoost(DateTime lastDailyFoodBoost) => 
            ChangeFoodBoost(1, false, lastDailyFoodBoost);

        public void ChooseRace(Race race) => 
            State.RaceState.Change(race);

        public void OpenNextBattle(int nextBattle) => 
            State.TimelineState.OpenNextBattle(nextBattle);

        public void SetAllBattlesWon(bool allBattlesWon) => 
            State.TimelineState.SetAllBattlesWon(true);

        public void UpgradeItem(UpgradeItemType type, float price)
        {
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

        private void ChangeFoodBoost(int delta, bool isPositive, DateTime lastDailyFoodBoost)
        {
            delta = isPositive ? delta : (delta * -1);
            State.FoodBoost.ChangeFoodBoostCount(delta, lastDailyFoodBoost);
        }

        private void ChangeAfterPurchase(float price, bool isPositive)
        {
            price = isPositive ? price : (price * -1);
            State.Currencies.ChangeCoins(price, isPositive);
        }
    }
}