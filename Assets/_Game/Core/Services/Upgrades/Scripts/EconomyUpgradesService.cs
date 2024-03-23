using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public class EconomyUpgradesService : IEconomyUpgradesService
    {
        public event Action<UpgradeItemViewModel> UpgradeItemUpdated;

        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;
        
        private readonly Dictionary<UpgradeItemType, UpgradeItemViewModel> _upgradeItems =
            new Dictionary<UpgradeItemType, UpgradeItemViewModel>(2);

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        public EconomyUpgradesService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _assetProvider = assetProvider;
            _logger = logger;
        }

        public async UniTask Init()
        {
            TimelineState.UpgradeItemChanged += OnUpgradeItemChanged;
            Currency.CoinsChanged += OnCoinsChanged;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            
            await PrepareUpgrades();
        }

        private async void OnNextAgeOpened()
        {
            await PrepareUpgrades();
        }

        public void OnUpgradesWindowOpened()
        {
            UpgradeItemUpdated?.Invoke(_upgradeItems[UpgradeItemType.FoodProduction]);
            UpgradeItemUpdated?.Invoke(_upgradeItems[UpgradeItemType.BaseHealth]);

            //TODO Delete
            _logger.Log("Upgrades updated");
        }

        private void OnCoinsChanged(bool isPositive)
        {
            var foodItem = _upgradeItems[UpgradeItemType.FoodProduction];
            foodItem.CanAfford = Currency.Coins > foodItem.Price;
            UpgradeItemUpdated?.Invoke(foodItem);
            
            var baseHealth = _upgradeItems[UpgradeItemType.BaseHealth];
            baseHealth.CanAfford = Currency.Coins > baseHealth.Price;
            UpgradeItemUpdated?.Invoke(baseHealth);
        }

        private void OnUpgradeItemChanged(UpgradeItemType type)
        {
            var model = _upgradeItems[type];
            float amount = 0; 
            
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    amount = GetFoodProductionSpeed();
                    model.Amount = amount;
                    model.AmountText = amount.ToSpeedFormat();
                    model.Price = GetFoodProductionPrice();
                    break;
                case UpgradeItemType.BaseHealth:
                    amount = GetBaseHealth();
                    model.Amount = amount;
                    model.AmountText = amount.FormatMoney();
                    model.Price = GetBaseHealthUpgradePrice();
                    break;
            }
            
            //TODO Delete 
            _logger.Log($"OnUpgradeItemChanged {type}");
            
            model.CanAfford = model.Price <= Currency.Coins;
            
            UpgradeItemUpdated?.Invoke(model);
        }

        public float GetFoodProductionSpeed()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();

            float initialSpeed = foodProduction.Speed;

            float totalSpeed = initialSpeed + foodProduction.SpeedStep * TimelineState.FoodProductionLevel;

            return totalSpeed;
        }

        private float GetFoodProductionPrice()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();

            float initialPrice = foodProduction.Price;
            
            float totalPrice = initialPrice + foodProduction.PriceExponential.GetValue(TimelineState.FoodProductionLevel); 
            
            totalPrice = Mathf.Round(totalPrice);
            
            //TODO Delete 
            _logger.Log($"INITIAL PRICE {initialPrice},  LEVEL {TimelineState.FoodProductionLevel}, TOTAL PRICE {totalPrice}");

            return totalPrice;
        }

        public int GetInitialFoodAmount()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();
            return foodProduction.InitialFoodAmount;
        }

        public float GetBaseHealth()
        {
            var healthConfig = _gameConfigController.GetBaseHealthConfig();
            float startHealth = healthConfig.Health;

            float totalHealth = startHealth + (healthConfig.HealthStep * TimelineState.BaseHealthLevel);
            
            return totalHealth;
        }

        private float GetBaseHealthUpgradePrice()
        {
            var healthConfig = _gameConfigController.GetBaseHealthConfig();
            float initialPrice = healthConfig.Price;
            
            float totalPrice = initialPrice + healthConfig.PriceExponential.GetValue(TimelineState.BaseHealthLevel);

            totalPrice = Mathf.Round(totalPrice);
            
            return totalPrice;
        }

        public void UpgradeItem(UpgradeItemType type)
        {
            float price = 0;
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    price = GetFoodProductionPrice();
                    break;
                case UpgradeItemType.BaseHealth:
                    price = GetBaseHealthUpgradePrice();
                    break;
            }
            if (Currency.Coins >= price)
            {
                _persistentData.UpgradeItem(type, price);
            }
        }

        private async UniTask PrepareUpgrades()
        {
            var ct = _cts.Token;
            try
            {
                _upgradeItems[UpgradeItemType.FoodProduction] = await PrepareFoodUpgradeItem(ct);
                _upgradeItems[UpgradeItemType.BaseHealth] = await PrepareBaseUpgradeItem(ct);

                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private async UniTask<UpgradeItemViewModel> PrepareFoodUpgradeItem(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var icon = await _assetProvider.Load<Sprite>(_gameConfigController.GetFoodIconKey()); 
            var price = GetFoodProductionPrice();

            var amount = GetFoodProductionSpeed();
            
            return new UpgradeItemViewModel
            {
                Type = UpgradeItemType.FoodProduction,
                Name = "Food production",
                Amount = amount,
                AmountText = amount.ToSpeedFormat(),
                Price = price,
                CanAfford = Currency.Coins >= price,
                Icon = icon,
            };
        }

        private async UniTask<UpgradeItemViewModel> PrepareBaseUpgradeItem(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var icon = await _assetProvider.Load<Sprite>(_gameConfigController.GetBaseIconKey());
            var price = GetBaseHealthUpgradePrice();

            var health = GetBaseHealth();
            
            return new UpgradeItemViewModel
            {
                Type = UpgradeItemType.BaseHealth,
                Name = "Base health",
                Amount = health,
                AmountText = health.FormatMoney(),
                Price = price,
                CanAfford = Currency.Coins >= price,
                Icon = icon,
            };
        }
    }
}