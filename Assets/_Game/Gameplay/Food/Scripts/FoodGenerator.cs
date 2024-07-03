using System;
using Assets._Game.Core._Logger;
using Assets._Game.Core._SystemUpdate;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Core.Data.Age.Dynamic._UpgradeItem;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services._FoodBoostService.Scripts;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay.Battle.Scripts;
using Assets._Game.UI.UnitBuilderBtn.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace Assets._Game.Gameplay.Food.Scripts
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

    public class FoodGenerator : IFoodGenerator, IGameUpdate, IBattleSpeedHandler, IDisposable
    {
        public event Action<int> FoodChanged;
        
        private readonly IMyLogger _logger;
        private readonly IFoodBoostService _foodBoostService;
        private readonly IPauseManager _pauseManager;
        private readonly IBattleSpeedManager _speedManager;
        private readonly ISystemUpdate _systemUpdate;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IEconomyConfigRepository _economyConfig;
        private readonly IUserContainer _userContainer;

        private IUpgradeItemsReadonly UpgradeItems => _generalDataPool.AgeDynamicData.UpgradeItems;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;

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
            IEconomyConfigRepository economyConfig,
            IMyLogger logger,
            GameplayUI gameplayUI,
            IFoodBoostService foodBoostService,
            ISystemUpdate systemUpdate,
            IPauseManager pauseManager,
            IBattleSpeedManager speedManager,
            IGeneralDataPool generalDataPool,
            IUserContainer userContainer)
        {
            _economyConfig = economyConfig;
            _panel = gameplayUI.FoodPanel;
            _logger = logger;
            _foodBoostService = foodBoostService;
            _speedManager = speedManager;
            _systemUpdate = systemUpdate;
            _generalDataPool = generalDataPool;
            _userContainer = userContainer;
            
            _pauseManager = pauseManager;
        }

        public void Init() => 
            UpgradeItems.Changed += UpdateGeneratorData;

        void IDisposable.Dispose() => 
            UpgradeItems.Changed -= UpdateGeneratorData;

        public void StartGenerator()
        {
            _panel.SetupIcon(_generalDataPool.AgeStaticData.ForFoodIcon(RaceState.CurrentRace));
            
            _systemUpdate.Register(this);
            _speedManager.Register(this);
            
            FoodChanged += _panel.OnFoodChanged;
            _foodBoostService.FoodBoost += AddFood;
            
            _defaultProductionSpeed = UpgradeItems.GetItemData(UpgradeItemType.FoodProduction).Amount;
            _productionSpeed = _defaultProductionSpeed * _speedManager.CurrentSpeedFactor;
            
            FoodAmount = _economyConfig.GetInitialFoodAmount();

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

        public void SetMediator(IBattleMediator battleMediator) => _battleMediator = battleMediator;

        private void UpdateGeneratorData(UpgradeItemType type, UpgradeItemDynamicData data)
        {
            if (type == UpgradeItemType.FoodProduction)
            {
                _productionSpeed = data.Amount;
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

        public void AddFood(int delta) => FoodAmount += delta;

        public void SpendFood(int delta) => FoodAmount -= delta;

        public void SetFactor(float speedFactor) => 
            _productionSpeed = _defaultProductionSpeed * speedFactor;
    }
}