using System;
using _Game.Core._Logger;
using _Game.Core._SystemUpdate;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI.GameplayUI.Scripts;
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

    public class FoodGenerator : IFoodGenerator, IGameUpdate
    {
        public event Action<int> FoodChanged;

        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IAgeStateService _ageState;
        private readonly IMyLogger _logger;
        private readonly IBonusRewardService _bonusRewardService;
        private readonly IPauseManager _pauseManager;
        
        private IBattleMediator _battleMediator;

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
            IBonusRewardService bonusRewardService,
            ISystemUpdate systemUpdate,
            IPauseManager pauseManager)
        {
            _economyUpgradesService = economyUpgradesService;
            _panel = gameplayUI.FoodPanel;
            _ageState = ageState;
            _logger = logger;
            _bonusRewardService = bonusRewardService;

            _pauseManager = pauseManager;
            systemUpdate.Register(this);
        }

        public void Init()
        {
            _panel.SetupIcon(_ageState.GetCurrentFoodIcon);
            _economyUpgradesService.UpgradeItemUpdated += UpdateGeneratorData;
        }

        public void StartGenerator()
        {
            FoodChanged += _panel.OnFoodChanged;
            _bonusRewardService.FoodBoost += AddFood;
            
            _productionSpeed = _economyUpgradesService.GetFoodProductionSpeed();
            FoodAmount = _economyUpgradesService.GetInitialFoodAmount();

            _panel.UpdateFillAmount(0);
            _panel.OnFoodChanged(FoodAmount);
        }

        public void StopGenerator()
        {
            FoodChanged -= _panel.OnFoodChanged;
            _bonusRewardService.FoodBoost -= AddFood;
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
    }
}