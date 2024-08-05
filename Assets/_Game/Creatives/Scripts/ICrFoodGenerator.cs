using System;

namespace _Game.Creatives.Scripts
{
    public interface ICrFoodGenerator
    {
        event Action<int> FoodChanged;
        int FoodAmount { get; set; }
        void SpendFood(int foodPrice);
        void StartGenerator();
    }
}