﻿using _Game.Core.Configs.Models;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Services.PersistentData
{
    public interface IPersistentDataService 
    {
        UserAccountState State { get; set; }
        GameConfig GameConfig { get; set; }
        void AddCoins(in float award);
        void PurchaseUnit(UnitType type, float price);
        void OpenNextBattle(int currentBattle);
        void SetAllBattlesWon(bool allBattlesWon);
        void UpgradeItem(UpgradeItemType type, float price);
        void OpenNextAge();
        void OpenNextTimeline();
    }
}