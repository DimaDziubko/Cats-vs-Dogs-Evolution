using System;
using _Game.Core._SystemUpdate;
using _Game.Core.Pause.Scripts;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.UnitBuilderBtn.Scripts;
using UnityEngine;

namespace _Game.Creatives.Scripts
{
    public class CrFoodGenerator : IFoodGenerator, IGameUpdate, IBattleSpeedHandler
    {
        public event Action<int> FoodChanged;
    
        private int _foodAmount;
    
        private readonly IPauseManager _pauseManager;
        private readonly ISystemUpdate _systemUpdate;
        private readonly IBattleSpeedManager _speedManager;
        private readonly FoodPanel _panel;
        private float _defaultProductionSpeed;
        private float _productionSpeed;
        private float _accumulatedTime;

        private bool IsPaused => _pauseManager.IsPaused;
    
        public int FoodAmount
        {
            get => _foodAmount;
            set
            {
                _foodAmount = value;
                FoodChanged?.Invoke(FoodAmount);
            }
        }

        public CrFoodGenerator(            
            GameplayUI gameplayUI,
            ISystemUpdate systemUpdate,
            IPauseManager pauseManager,
            IBattleSpeedManager speedManager)
        {
            _panel = gameplayUI.FoodPanel;
            _speedManager = speedManager;
            _systemUpdate = systemUpdate;
            _pauseManager = pauseManager;
        }
    
        public void Init()
        {
        
        }

        public void AddFood(int delta)
        {
            FoodAmount += delta;
        }

        public void SpendFood(int delta)
        {
            FoodAmount -= delta;
        }

        public void StartGenerator()
        {
            _systemUpdate.Register(this);
            _speedManager.Register(this);
            
            FoodChanged += _panel.OnFoodChanged;

            _defaultProductionSpeed =  CrSceneContext.I.FoodProductionSpeed;
            _productionSpeed = _defaultProductionSpeed * _speedManager.CurrentSpeedFactor;

            FoodAmount = CrSceneContext.I.InitialFood;

            _panel.UpdateFillAmount(0);
            _panel.OnFoodChanged(FoodAmount);
        }

        public void StopGenerator()
        {
            _accumulatedTime = 0;
            FoodChanged -= _panel.OnFoodChanged;
            _systemUpdate.Unregister(this);
            _speedManager.UnRegister(this);
        }

        public void SetMediator(IBattleMediator battleMediator)
        {
        
        }

        void IGameUpdate.GameUpdate()
        {
            if(IsPaused || !CrQuickGame.I.BattleInProcess) return;

            _accumulatedTime += Time.deltaTime;
            float foodProgress = _accumulatedTime * _productionSpeed;

            if (foodProgress >= 1f) 
            {
                FoodAmount += (int)foodProgress; 
                _accumulatedTime = 0f; 
            }
        
            _panel.UpdateFillAmount(foodProgress % 1);
        }

        public void SetFactor(float speedFactor)
        {
            _productionSpeed = _defaultProductionSpeed * speedFactor;
        }
    }
}
