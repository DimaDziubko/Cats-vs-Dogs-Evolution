using _Game.Core.Configs.Models;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Services.UserContainer
{
    public interface IUserContainer 
    {
        UserAccountState State { get; set; }
        GameConfig GameConfig { get; set; }
        void AddCoins(in float award);
        void PurchaseUnit(UnitType type, float price);
        void OpenNextBattle(int currentBattle);
        void SetAllBattlesWon(bool allBattlesWon);
        void UpgradeItem(UpgradeItemType type, float price);
        void OnOpenNextAge();
        void OpenNextTimeline();
        void RecoverFoodBoost(int dailyFoodBoostCount);
        void SpendFoodBoost();
        void ChooseRace(Race race);
        void AddCompletedBattle();
        void CompleteTutorialStep(int tutorialDataStep);
        void ChangeNormalSpeed(bool isNormal);
        void ChangeBattleTimerDurationLeft(float timerTimeLeft);
        void AddAdsReviewed();
        void FirstDayRetentionSent();
        void SecondDayRetentionSent();
    }
}