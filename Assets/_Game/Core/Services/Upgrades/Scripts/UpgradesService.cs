using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public class UpgradesService : IUpgradesService
    {
        public event Action<List<UpgradeUnitItemModel>> UpgradeUnitItemsUpdated;
        
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        //TODO Update after award
        private readonly List<UpgradeUnitItemModel> _upgradeUnitItems = new List<UpgradeUnitItemModel>(3);
        
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        public UpgradesService(
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
            TimelineState.OpenedUnit += OnUnitOpened;
            await PrepareUpgrades();
        }
        
        public async UniTask PrepareUpgrades()
        {
            List<WarriorConfig> openPlayerUnitConfigs = _gameConfigController.GetOpenPlayerUnitConfigs();
            _logger.Log($"OpenPlayerUnits : {openPlayerUnitConfigs.Count}");

            var ct = _cts.Token;
            try
            {
                await PrepareUpgradeItems(ct);
                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        public List<UpgradeUnitItemModel> GetUpgradeUnitItems()
        {
            if (_upgradeUnitItems != null)
            {
                return _upgradeUnitItems;
            }
            else
            {
                _logger.Log("UnitBuilderData data is empty");
                return null;
            }
        }

        public void PurchaseUnit(UnitType type)
        {
            var unitPrice = _gameConfigController.GetUnitPrice(type);
            
            if (Currency.Coins >= unitPrice)
            {
                _persistentData.PurchaseUnit(type, unitPrice);
            }
            else
            {
                Debug.LogError($"Cannot purchase unit {type}. Either already opened or not enough coins.");
            }
        }

        public float GetFoodProductionSpeed()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();

            float baseSpeed = foodProduction.Speed;

            float totalSpeed = baseSpeed * Mathf.Pow(foodProduction.SpeedFactor, TimelineState.FoodProductionLevel);
    
            return totalSpeed;
        }

        public int GetInitialFoodAmount()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();
            return foodProduction.InitialFoodAmount;
        }

        private void OnUnitOpened(UnitType type)
        {
            UpdateUnitItem(type);
            UpgradeUnitItemsUpdated?.Invoke(_upgradeUnitItems);
        }

        private void UpdateUnitItem(UnitType type)
        {
            var model = _upgradeUnitItems.FirstOrDefault(x => x.Type == type);
            if (model != null) model.IsBought = true;
        }

        private async UniTask PrepareUpgradeItems(CancellationToken ct)
        {
            List<WarriorConfig> warriorConfigs = _gameConfigController.GetCurrentAgeUnits();

            _upgradeUnitItems.Clear();
            
            foreach (var config in warriorConfigs)
            {
                ct.ThrowIfCancellationRequested();
                
                Sprite icon = await _assetProvider.Load<Sprite>(config.IconKey);

                var item = new UpgradeUnitItemModel
                {
                    Type = config.Type,
                    Icon = icon, 
                    Name = config.Name,
                    Price = config.Price,
                    IsBought = TimelineState.OpenUnits.Contains(config.Type),
                    CanAfford = Currency.Coins >= config.Price
                };
                
                _upgradeUnitItems.Add(item);
            }
        }
    }
}