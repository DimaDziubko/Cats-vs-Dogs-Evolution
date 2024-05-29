using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Pin.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Upgrades.Scripts
{
    public class UnitUpgradesService : IUnitUpgradesService, IDisposable 
    {
        public event Action<List<UpgradeUnitItemViewModel>> UpgradeUnitItemsUpdated;
        
        private readonly IPersistentDataService _persistentData;
        private readonly IAgeConfigRepository _ageConfigRepository;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        private readonly List<UpgradeUnitItemViewModel> _upgradeUnitItems = new List<UpgradeUnitItemViewModel>(3);

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public UnitUpgradesService(
            IPersistentDataService persistentData,
            IAgeConfigRepository ageConfigRepository,
            IAssetProvider assetProvider,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _persistentData = persistentData;
            _ageConfigRepository = ageConfigRepository;
            _assetProvider = assetProvider;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
        }

        public async UniTask Init()
        {
            UpgradeUnitItemsUpdated += _upgradesChecker.OnUpgradeUnitItemsUpdated;
            TimelineState.OpenedUnit += OnUnitOpened;
            Currency.CoinsChanged += OnCoinsChanged;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            RaceState.Changed += OnRaceChanged;

            await PrepareUpgrades();
        }

        public void Dispose()
        {
            _cts?.Dispose();
            UpgradeUnitItemsUpdated -= _upgradesChecker.OnUpgradeUnitItemsUpdated;
            TimelineState.OpenedUnit -= OnUnitOpened;
            Currency.CoinsChanged -= OnCoinsChanged;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            RaceState.Changed -= OnRaceChanged;
        }

        private async void OnRaceChanged() => await PrepareUpgrades();

        private async void OnNextAgeOpened() => 
            await PrepareUpgrades();

        public void PurchaseUnit(UnitType type)
        {
            float? unitPrice = _ageConfigRepository
                .GetAgeUnits(TimelineState.AgeId)
                .Where(x => x.Type == type)
                .Select(x => (float?)x.Price)
                .FirstOrDefault();
            
            if (unitPrice.HasValue && Currency.Coins >= unitPrice.Value)
            {
                _persistentData.PurchaseUnit(type, unitPrice.Value);
            }
            else
            {
                Debug.LogError($"Cannot purchase unit {type}. Not enough coins or unit not found.");
            }
        }

        private async UniTask PrepareUpgrades()
        {
            var ct = _cts.Token;
            try
            {
                await PrepareUnitUpgradeItems(ct);
                UpgradeUnitItemsUpdated?.Invoke(_upgradeUnitItems);

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
            IEnumerable<WarriorConfig> warriorConfigs = _ageConfigRepository.GetAgeUnits(TimelineState.AgeId);

            _upgradeUnitItems.Clear();
            
            foreach (var config in warriorConfigs)
            {
                ct.ThrowIfCancellationRequested();

                string iconKey = config.GetUnitIconKeyForRace(RaceState.CurrentRace);
                
                Sprite icon = await _assetProvider.Load<Sprite>(iconKey);

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

        void IUnitUpgradesService.OnUpgradesWindowOpened() => 
            UpgradeUnitItemsUpdated?.Invoke(_upgradeUnitItems);
    }
}