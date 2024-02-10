using System;
using _Game.Gameplay.UpgradesAndEvolution.Scripts;
using UnityEngine;

namespace _Game.Gameplay.Food.Scripts
{
    public class FoodGenerator
    {
        public event Action<int> FoodChanged;
        public event Action<float> FoodProgressUpdated;

        private readonly IUpgradesAndEvolutionService _upgradesAndEvolutionService;

        private float _productionSpeed;
        private int _foodAmount;
        private float _accumulatedTime;

        public int FoodAmount
        {
            get => _foodAmount;
            set
            {
                _foodAmount = value;
                FoodChanged?.Invoke(FoodAmount);
            }
        }

        public FoodGenerator(IUpgradesAndEvolutionService upgradesAndEvolutionService)
        {
            _upgradesAndEvolutionService = upgradesAndEvolutionService;
        }

        public void Init()
        {
            _productionSpeed = _upgradesAndEvolutionService.GetFoodProductionSpeed();
        }

        public void GameUpdate()
        {
            _accumulatedTime += Time.deltaTime;
            float foodProgress = _accumulatedTime * _productionSpeed;

            if (foodProgress >= 1f) 
            {
                FoodAmount += (int)foodProgress; 
                _accumulatedTime = 0f; 
            }
        
            FoodProgressUpdated?.Invoke(foodProgress % 1);
        }

        public void AddFood(int delta)
        {
            FoodAmount += delta;
        }
        
        public void SpendFood(int delta)
        {
            FoodAmount -= delta;
        }
    }
}