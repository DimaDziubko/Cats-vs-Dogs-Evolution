using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public class UnitUpgradesService : IUnitUpgradesService 
    {
        public event Action<List<UpgradeUnitItemViewModel>> UpgradeUnitItemsUpdated;
        
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;
        
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;
        
        private readonly List<UpgradeUnitItemViewModel> _upgradeUnitItems = new List<UpgradeUnitItemViewModel>(3);
        
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        public UnitUpgradesService(
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
            Currency.CoinsChanged += OnCoinsChanged;

            TimelineState.NextAgeOpened += OnNextAgeOpened;
            
            await PrepareUpgrades();
        }

        private async void OnNextAgeOpened()
        {
            await PrepareUpgrades();
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

        private async UniTask PrepareUpgrades()
        {
            List<WarriorConfig> openPlayerUnitConfigs = _gameConfigController.GetOpenPlayerUnitConfigs();
            _logger.Log($"OpenPlayerUnits : {openPlayerUnitConfigs.Count}");

            var ct = _cts.Token;
            try
            {
                await PrepareUnitUpgradeItems(ct);

                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
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

        private async UniTask PrepareUnitUpgradeItems(CancellationToken ct)
        {
            List<WarriorConfig> warriorConfigs = _gameConfigController.GetCurrentAgeUnits();

            _upgradeUnitItems.Clear();
            
            foreach (var config in warriorConfigs)
            {
                ct.ThrowIfCancellationRequested();
                
                Sprite icon = await _assetProvider.Load<Sprite>(config.IconKey);

                var item = new UpgradeUnitItemViewModel
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

        private void OnCoinsChanged(bool isPositive)
        {
            foreach (var unitItem in _upgradeUnitItems)
            {
                unitItem.CanAfford = Currency.Coins > unitItem.Price;
            }
            
            UpgradeUnitItemsUpdated?.Invoke(_upgradeUnitItems);
        }
        

        public void OnUpgradesWindowOpened()
        {
            UpgradeUnitItemsUpdated?.Invoke(_upgradeUnitItems);
            
            //TODO Delete
            _logger.Log("Upgrades updated");
        }
        
    }
}