using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.DataProviders;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Age.Scripts
{
    public sealed class AgeStateService : IAgeStateService
    {
        public event Action RaceChangingBegun;
        public event Action AgeUpdated;
        public event Action<BaseData> BaseDataUpdated;
        public event Action<UnitBuilderBtnData[]> BuilderDataUpdated;

        private readonly IPersistentDataService _persistentData;

        private readonly IGameConfigController _gameConfigController;

        private readonly IAssetRegistry _assetRegistry;

        private readonly IEconomyUpgradesService _economyUpgrades;

        private readonly IMyLogger _logger;

        private readonly IUnitDataProvider _unitDataProvider;
        private readonly IWeaponDataProvider _weaponDataProvider;
        private readonly IBaseDataProvider _baseDataProvider;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;

        private readonly Dictionary<UnitType, UnitData> _playerUnitData = 
            new Dictionary<UnitType, UnitData>(3);

        private readonly Dictionary<WeaponType, WeaponData> _weaponData = 
            new Dictionary<WeaponType, WeaponData>(3);

        private readonly UnitBuilderBtnData[] _unitBuilderData = new UnitBuilderBtnData[3];

        private BaseData _playerBaseData;

        public Sprite GetCurrentFoodIcon => _unitBuilderData[0].FoodIcon;

        public BaseData GetForPlayerBase() => _playerBaseData;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public AgeStateService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IAssetRegistry assetRegistry,
            IEconomyUpgradesService economyUpgrades,
            IMyLogger logger,
            IUnitDataProvider unitDataProvider,
            IWeaponDataProvider weaponDataProvider,
            IBaseDataProvider baseDataProvider)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _assetRegistry = assetRegistry;
            _economyUpgrades = economyUpgrades;
            _logger = logger;
            _unitDataProvider = unitDataProvider;
            _weaponDataProvider = weaponDataProvider;
            _baseDataProvider = baseDataProvider;
        }

        public async UniTask Init()
        {
            TimelineState.OpenedUnit += OnOpenedUnit;
            _economyUpgrades.UpgradeItemUpdated += OnUpgradeItemUpdated;
            TimelineState.NextAgeOpened += OnNextAgeOpened;

            await PrepareAge();
        }

        public async UniTask ChangeRace()
        {
            RaceChangingBegun?.Invoke();
            await PrepareAge();
            AgeUpdated?.Invoke();
        }

        public void ReleaseResources() => _assetRegistry.ClearContext(Constants.CacheContext.AGE);

        public void OnBuilderStarted() => 
            BuilderDataUpdated?.Invoke(_unitBuilderData);

        private async void OnNextAgeOpened()
        {
            await PrepareAge();
            
            BuilderDataUpdated?.Invoke(_unitBuilderData);
            
            AgeUpdated?.Invoke();
        }

        public WeaponData ForWeapon(WeaponType type)
        {
            if (_weaponData.TryGetValue(type, out var projectileData))
            {
                return projectileData;
            }
            else
            {
                _logger.Log("Projectile data is empty (age)");
                return null;
            }
        }

        private void OnUpgradeItemUpdated(UpgradeItemViewModel model)
        {
            if (model.Type == UpgradeItemType.BaseHealth)
            {
                UpdateBaseDataHealth(model.Amount);
            }
        }

        private void UpdateBaseDataHealth(float amount)
        {
            _playerBaseData.Health = amount;
            BaseDataUpdated?.Invoke(_playerBaseData);
        }

        public UnitData ForPlayerUnit(UnitType type)
        {
            if (_playerUnitData.TryGetValue(type, out var unitData))
            {
                return unitData;
            }
            else
            {
                _logger.Log("Player data is empty");
                return null;
            }
        }

        private async void OnOpenedUnit(UnitType type)
        {
            var config = _gameConfigController.GetCurrentAgeUnits().FirstOrDefault(w => w.Type == type);
            if (config != null)
            {
                await AddPlayerUnitData(type, config);
                await AddUnitBuilderData(type, config);
                await AddWeaponData(type, config);
            }
        }

        private async UniTask PrepareAge()
        {
            List<WarriorConfig> openPlayerUnitConfigs = _gameConfigController.GetOpenPlayerUnitConfigs();
            
            _logger.Log($"OpenPlayerUnits : {openPlayerUnitConfigs.Count}");

            var ct = _cts.Token;
            try
            {
                Cleanup();
                ReleaseResources();

                await PreparePlayerUnitsData(openPlayerUnitConfigs, ct);
                await PrepareWeaponsData(openPlayerUnitConfigs, ct);
                await PrepareUnitsBuilderData(openPlayerUnitConfigs, ct);
                await PreparePlayerBaseData(ct);

                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private async UniTask PreparePlayerUnitsData(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            _playerUnitData.Clear();
            
            foreach (var config in openPlayerUnitConfigs)
            {
                await PreparePlayerUnitData(config, ct);
            }
            
            _logger.Log("PlayerUnitData prepared");
        }

        private async UniTask PreparePlayerUnitData(WarriorConfig config, CancellationToken ct)
        {
            var unitLoadOption = new UnitLoadOptions()
            {
                Faction = Faction.Player, 
                Config = config,
                CacheContext = Constants.CacheContext.AGE,
                CurrentRace = RaceState.CurrentRace,
                CancellationToken = ct
            };
            
            _playerUnitData[config.Type] =
                await _unitDataProvider.LoadUnitData(unitLoadOption);
        }

        private async UniTask AddPlayerUnitData(UnitType type, WarriorConfig config)
        {
            var ct = _cts.Token;                
            try
            {
                await PreparePlayerUnitData(config, ct);
                
                _logger.Log($"Added new unit data for {type}.");
            }
            catch (Exception e)
            {
                _logger.Log("AddPlayerUnitData was canceled.");
            }
        }
        
        private async UniTask PreparePlayerBaseData(CancellationToken ct)
        {
            var ageConfig = _gameConfigController.GetAgeConfig(TimelineState.AgeId);
            string key = ageConfig.GetBaseKeyForRace(RaceState.CurrentRace);
            
            var baseLoadOptions = new BaseLoadOptions()
            {
                Faction = Faction.Player,
                CacheContext = Constants.CacheContext.AGE,
                PrefabKey = key,
                Health = _economyUpgrades.GetBaseHealth(),
                CoinsAmount = 0,
                CancellationToken = ct,
            };
            
            _playerBaseData = await _baseDataProvider.LoadBase(baseLoadOptions);
        }

        private async UniTask PrepareUnitsBuilderData(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            foreach (var config in openPlayerUnitConfigs)
            {
                await PrepareUnitBuilderData(ct, config);
            }

            _logger.Log($"UnitBuilderData prepared {_unitBuilderData.Length}");
        }

        private async UniTask PrepareUnitBuilderData(CancellationToken ct, WarriorConfig config)
        {
            var builderLoadOptions = new BuilderLoadOptions()
            {
                Config = config,
                CurrentRace = RaceState.CurrentRace,
                CancellationToken = ct
            };

            _unitBuilderData[(int) config.Type] =
                await _unitDataProvider.LoadUnitBuilderData(builderLoadOptions);
        }

        private async UniTask PrepareWeaponsData(List<WarriorConfig> warriorConfigs, CancellationToken ct)
        {
            _weaponData.Clear();
    
            foreach (var config in warriorConfigs)
            {
                await PrepareWeaponData(ct, config.WeaponConfig);
            }

            _logger.Log($"WeaponData prepared {_weaponData.Count}");
        }

        private async UniTask AddWeaponData(UnitType type, WarriorConfig config)
        {
            var ct = _cts.Token;                
            try
            {
                await PrepareWeaponData(ct, config.WeaponConfig);
                
                _logger.Log($"Added new weapon data for {type}.");
            }
            catch (Exception e)
            {
                _logger.Log("AddWeapon was canceled.");
            }
        }

        private async UniTask AddUnitBuilderData(UnitType type, WarriorConfig config)
        {
            var ct = _cts.Token;                
            try
            {
                await PrepareUnitBuilderData(ct, config);
                
                _logger.Log($"Added new unit builder data for {type}.");
            }
            catch (Exception e)
            {
                _logger.Log("AddUnitBuilderData was canceled.");
            }
        }

        private async UniTask PrepareWeaponData(CancellationToken ct, WeaponConfig config)
        {
            if (config.WeaponType == WeaponType.Melee) return;

            var weaponLoadOptions = new WeaponLoadOptions()
            {
                Faction = Faction.Player,
                Config = config,
                CacheContext = Constants.CacheContext.AGE,
                CancellationToken = ct
            };
            
            _weaponData[config.WeaponType] =
                await _weaponDataProvider.LoadWeapon(weaponLoadOptions);
        }


        private void Cleanup()
        {
            for (int i = 0; i < _unitBuilderData.Length; i++)
            {
                _unitBuilderData[i] = null;
            }
            _playerUnitData.Clear();
            _weaponData.Clear();
        }
    }
}