using System;
using _Game.UI.Hud;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.BonusReward.Scripts
{
    public interface IBonusRewardService
    {
        UniTask Init();
        
        event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        event Action<int> FoodBoost;
        public void OnHudOpened();
        void OnFoodBoostBtnClicked();
    }
}