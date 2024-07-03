using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace Assets._Game.Core.Services.UserContainer
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