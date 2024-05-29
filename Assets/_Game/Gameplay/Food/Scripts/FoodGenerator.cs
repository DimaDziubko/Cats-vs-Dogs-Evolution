using System;
using _Game.Core._Logger;
using _Game.Core._SystemUpdate;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI.UnitBuilderBtn.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Gameplay.Food.Scripts
{
    public interface IFoodGenerator
    {
        event Action<int> FoodChanged;
        int FoodAmount { get; set; }
        public void Init();
        public void AddFood(int delta);
        public void SpendFood(int delta);
        void StartGenerator();
        void StopGenerator();
        void SetMediator(IBattleMediator battleMediator);
    }

    public class FoodGenerator : IFoodGenerator, IGameUpdate, IBattleSpeedHandler
    {
        public event Action<int> FoodChanged;

        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IAgeStateService _ageState;
        private readonly IMyLogger _logger;
        private readonly IFoodBoostService _foodBoostService;
        private readonly IPauseManager _pauseManager;
        private readonly IBattleSpeedManager _speedManager;
        private readonly ISystemUpdate _systemUpdate;

        private IBattleMediator _battleMediator;

        private float _defaultProductionSpeed;
        private bool IsPaused => _pauseManager.IsPaused;

        private float _productionSpeed;
        private int _foodAmount;
        private float _accumulatedTime;

        private readonly FoodPanel _panel;
        
        public int FoodAmount
        {
            get => _foodAmount;
            set
            {
                _foodAmount = value;
                FoodChanged?.Invoke(FoodAmount);
            }
        }

        public FoodGenerator(
            IEconomyUpgradesService economyUpgradesService, 
            IAgeStateService ageState,
            IMyLogger logger,
            GameplayUI gameplayUI,
            IFoodBoostService foodBoostService,
            ISystemUpdate systemUpdate,
            IPauseManager pauseManager,
            IBattleSpeedManager speedManager)
        {
            _economyUpgradesService = economyUpgradesService;
            _panel = gameplayUI.FoodPanel;
            _ageState = ageState;
            _logger = logger;
            _foodBoostService = foodBoostService;
            _speedManager = speedManager;
            _systemUpdate = systemUpdate;
            
            _pauseManager = pauseManager;
        }

        public void Init()
        {
            _panel.SetupIcon(_ageState.GetCurrentFoodIcon);
            _economyUpgradesService.UpgradeItemUpdated += UpdateGeneratorData;
        }

        public void StartGenerator()
        {
            _systemUpdate.Register(this);
            _speedManager.Register(this);
            
            FoodChanged += _panel.OnFoodChanged;
            _foodBoostService.FoodBoost += AddFood;
            
            _defaultProductionSpeed =  _economyUpgradesService.GetFoodProductionSpeed();
            _productionSpeed = _defaultProductionSpeed * _speedManager.CurrentSpeedFactor;
            
            FoodAmount = _economyUpgradesService.GetInitialFoodAmount();

            _panel.UpdateFillAmount(0);
            _panel.OnFoodChanged(FoodAmount);
        }

        public void StopGenerator()
        {
            _accumulatedTime = 0;
            FoodChanged -= _panel.OnFoodChanged;
            _foodBoostService.FoodBoost -= AddFood;
            _systemUpdate.Unregister(this);
            _speedManager.UnRegister(this);
        }

        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }

        private void UpdateGeneratorData(UpgradeItemViewModel model)
        {
            if (model.Type == UpgradeItemType.FoodProduction)
            {
                _productionSpeed = model.Amount;
            }
        }
        
        void IGameUpdate.GameUpdate()
        {
            if(IsPaused || !_battleMediator.BattleInProcess) return;

            _accumulatedTime += Time.deltaTime;
            float foodProgress = _accumulatedTime * _productionSpeed;

            if (foodProgress >= 1f) 
            {
                FoodAmount += (int)foodProgress; 
                _accumulatedTime = 0f; 
            }
        
            _panel.UpdateFillAmount(foodProgress % 1);
        }

        public void AddFood(int delta)
        {
            FoodAmount += delta;
        }
        
        public void SpendFood(int delta)
        {
            FoodAmount -= delta;
        }

        public void SetFactor(float speedFactor)
        {
            _productionSpeed = _defaultProductionSpeed * speedFactor;
        }
    }
}