using System;
using _Game.UI._Hud;

namespace Assets._Game.Core.Services._FoodBoostService.Scripts
{
    public interface IFoodBoostService
    {
        event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        event Action<int> FoodBoost;
        public void OnFoodBoostShown();
        void OnFoodBoostBtnClicked();
    }
}