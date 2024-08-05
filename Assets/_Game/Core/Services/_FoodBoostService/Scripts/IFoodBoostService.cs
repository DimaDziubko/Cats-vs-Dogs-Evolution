using System;
using _Game.UI._Hud;

namespace _Game.Core.Services._FoodBoostService.Scripts
{
    public interface IFoodBoostService
    {
        event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        public void OnFoodBoostShown();
        void OnFoodBoostBtnClicked();
    }
}