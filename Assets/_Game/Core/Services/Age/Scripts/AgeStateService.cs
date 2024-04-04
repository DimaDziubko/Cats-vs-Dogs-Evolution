using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Age.Scripts
{
    public sealed class AgeStateService : IAgeStateService
    {
        public event Action AgeUpdated;
        public event Action<BaseData> BaseDataUpdated;
        public event Action<UnitBuilderBtnData[]> BuilderDataUpdated;

        private readonly IPersistentDataService _persistentData;

        private readonly IGameConfigController _gameConfigController;

        private readonly IAssetProvider _assetProvider;

        private readonly IEconomyUpgradesService _economyUpgrades;

        private readonly IMyLogger _logger;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;

        private readonly HashSet<string> _loadedResources = new HashSet<string>();
        
        private readonly Dictionary<UnitType, UnitData> _playerUnitData = 
            new Dictionary<UnitType, UnitData>(3);

        private readonly Dictionary<WeaponType, WeaponData> _weaponData = 
            new Dictionary<WeaponType, WeaponData>(3);

        private readonly UnitBuilderBtnData[] _unitBuilderData = new UnitBuilderBtnData[3];

        private BaseData _playerBaseData;

        public Sprite GetCurrentFoodIcon { get; private set; }

        public BaseData GetForPlayerBase() => _playerBaseData;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public AgeStateService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IAssetProvider assetProvider,
            IEconomyUpgradesService economyUpgrades,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _assetProvider = assetProvider;
            _economyUpgrades = economyUpgrades;
            _logger = logger;
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
            await PrepareAge();
            AgeUpdated?.Invoke();
        }

        public void OnBuilderStarted() => 
            BuilderDataUpdated?.Invoke(_unitBuilderData);

        private async void OnNextAgeOpened()
        {
            await PrepareAge();
            
            BuilderDataUpdated?.Invoke(_unitBuilderData);
            
            AgeUpdated?.Invoke();
        }

        private void OnUpgradeItemUpdated(UpgradeItemViewModel model)
        {
            if (model.Type == UpgradeItemType.BaseHealth)
            {
                UpdateBaseHealth(model.Amount);
            }
        }

        private void UpdateBaseHealth(float amount)
        {
            _playerBaseData.Health = amount;
            BaseDataUpdated?.Invoke(_playerBaseData);
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

        public UnitData GetPlayerUnit(UnitType type)
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
        
        private async UniTask PrepareAge()
        {
            List<WarriorConfig> openPlayerUnitConfigs = _gameConfigController.GetOpenPlayerUnitConfigs();
            
            _logger.Log($"OpenPlayerUnits : {openPlayerUnitConfigs.Count}");

            var ct = _cts.Token;
            try
            {
                Cleanup();
                UnloadResources();
                ClearKeyCache();
                
                await PreparePlayerUnits(openPlayerUnitConfigs, ct);
                await PrepareWeaponData(openPlayerUnitConfigs, ct);
                await PrepareUnitBuilderData(openPlayerUnitConfigs, ct);
                await PreparePlayerBaseData(ct);

                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private async UniTask PreparePlayerBaseData(CancellationToken ct)
        {
            var ageConfig = _gameConfigController.GetAgeConfig(TimelineState.AgeId);
            
            ct.ThrowIfCancellationRequested();

            string baseKey = ageConfig.GetBaseKeyForRace(RaceState.CurrentRace);
            
            var playerBasePrefab = await _assetProvider.Load<GameObject>(baseKey);
            
            RegisterKey(baseKey);
            
            _playerBaseData = new BaseData()
            {
                Health = _economyUpgrades.GetBaseHealth(),
                BasePrefab = playerBasePrefab.GetComponent<Base>(),
                Layer = Constants.Layer.PLAYER_BASE
            };
        }

        private async UniTask PrepareUnitBuilderData(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            var foodIconKey = _gameConfigController.GetFoodIconKey();
            
            ct.ThrowIfCancellationRequested();
            var foodSprite = await _assetProvider.Load<Sprite>(foodIconKey);
            
            GetCurrentFoodIcon = foodSprite;

            for (int i = 0; i < openPlayerUnitConfigs.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var config = openPlayerUnitConfigs[i];
                string iconKey = config.GetUnitIconKeyForRace(RaceState.CurrentRace);
                var unitIcon = await _assetProvider.Load<Sprite>(iconKey);

                RegisterKey(iconKey);
                
                var newData = new UnitBuilderBtnData
                {
                    Type = config.Type,
                    Food = foodSprite,
                    UnitIcon = unitIcon,
                    FoodPrice = config.FoodPrice,
                };
                
                _unitBuilderData[(int)config.Type] = newData; 
            }

            _logger.Log($"UnitBuilderData prepared {_unitBuilderData.Length}");
        }

        private async UniTask PreparePlayerUnits(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            _playerUnitData.Clear();
    
            foreach (var config in openPlayerUnitConfigs)
            {
                ct.ThrowIfCancellationRequested();

                var unitKey = config.GetUnitKeyForCurrentRace(RaceState.CurrentRace);
                
                var go = await _assetProvider.Load<GameObject>(unitKey);
                
                RegisterKey(unitKey);
                
                var newData = new UnitData
                {
                    Config = config,
                    Prefab = go.GetComponent<Unit>(),
                    UnitLayer = Constants.Layer.PLAYER,
                    AggroLayer = Constants.Layer.PLAYER_AGGRO,
                    AttackLayer = Constants.Layer.PLAYER_ATTACK,
                };

                _playerUnitData[config.Type] = newData;
            }

            _logger.Log("PlayerUnitData prepared");
        }
        
        private async UniTask PrepareWeaponData(List<WarriorConfig> warriorConfigs, CancellationToken ct)
        {
            _weaponData.Clear();
    
            foreach (var config in warriorConfigs)
            {
                ct.ThrowIfCancellationRequested();

                if(config.WeaponConfig.WeaponType == WeaponType.Melee) continue;

                Projectile projectilePrefab = await PrepareProjectilePrefab(config);
        
                MuzzleFlash muzzlePrefab = await PrepareMuzzleFlash(config);
        
                ProjectileExplosion projectileExplosionPrefab = await PrepareProjectileExplosionPrefab(config);

                var newData = new WeaponData()
                {
                    Config = config.WeaponConfig,
                    Layer = Constants.Layer.PLAYER_PROJECTILE,
                    ProjectilePrefab = projectilePrefab,
                    MuzzlePrefab = muzzlePrefab,
                    ProjectileExplosionPrefab = projectileExplosionPrefab,
                };

                _weaponData[config.WeaponConfig.WeaponType] = newData;
            }

            _logger.Log($"WeaponData prepared {_weaponData.Count}");
        }

        private async UniTask<ProjectileExplosion> PrepareProjectileExplosionPrefab(WarriorConfig config)
        {
            ProjectileExplosion projectileExplosionPrefab = null;
            if (config.WeaponConfig.ProjectileExplosionKey != Constants.ConfigKeys.MISSING_KEY)
            {
                var projectileExplosionGameObject =
                    await _assetProvider.Load<GameObject>(config.WeaponConfig.ProjectileExplosionKey);
                if (projectileExplosionGameObject != null)
                {
                    projectileExplosionPrefab = projectileExplosionGameObject.GetComponent<ProjectileExplosion>();
                    RegisterKey(config.WeaponConfig.ProjectileExplosionKey);
                }
            }

            return projectileExplosionPrefab;
        }

        private async UniTask<MuzzleFlash> PrepareMuzzleFlash(WarriorConfig config)
        {
            MuzzleFlash muzzlePrefab = null;
            if (config.WeaponConfig.MuzzleKey != Constants.ConfigKeys.MISSING_KEY)
            {
                var muzzleGameObject = await _assetProvider.Load<GameObject>(config.WeaponConfig.MuzzleKey);
                if (muzzleGameObject != null)
                {
                    muzzlePrefab = muzzleGameObject.GetComponent<MuzzleFlash>();
                    RegisterKey(config.WeaponConfig.MuzzleKey);
                }
            }
            return muzzlePrefab;
        }

        private async UniTask<Projectile> PrepareProjectilePrefab(WarriorConfig config)
        {
            Projectile projectilePrefab = null;
            if (config.WeaponConfig.ProjectileKey != Constants.ConfigKeys.MISSING_KEY)
            {
                var projectileGameObject = await _assetProvider.Load<GameObject>(config.WeaponConfig.ProjectileKey);
                if (projectileGameObject != null)
                {
                    projectilePrefab = projectileGameObject.GetComponent<Projectile>();
                    RegisterKey(config.WeaponConfig.ProjectileKey);
                }
            }
            
            return projectilePrefab;
        }


        private async UniTask AddWeaponData(UnitType type, WarriorConfig config)
        {
            if(config.WeaponConfig.WeaponType == WeaponType.Melee) return;
            
            Projectile projectilePrefab = await PrepareProjectilePrefab(config);
        
            MuzzleFlash muzzlePrefab = await PrepareMuzzleFlash(config);
        
            ProjectileExplosion projectileExplosionPrefab = await PrepareProjectileExplosionPrefab(config);
            
            
            var newData = new WeaponData()
            {
                Config = config.WeaponConfig,
                Layer = Constants.Layer.PLAYER_PROJECTILE,
                ProjectilePrefab = projectilePrefab,
                MuzzlePrefab = muzzlePrefab,
                ProjectileExplosionPrefab = projectileExplosionPrefab,
            };

            _weaponData[config.WeaponConfig.WeaponType] = newData;
            _logger.Log($"Added new weapon data for {type}.");
        }

        private async UniTask AddPlayerUnitData(UnitType type, WarriorConfig config)
        {
            var unitKey = config.GetUnitKeyForCurrentRace(RaceState.CurrentRace);
            
            var prefab = await _assetProvider.Load<GameObject>(unitKey);
            
            RegisterKey(unitKey);
            
            var unitData = new UnitData
            {
                Config = config,
                Prefab = prefab.GetComponent<Unit>(),
                UnitLayer = Constants.Layer.PLAYER,
                AggroLayer = Constants.Layer.PLAYER_AGGRO,
                AttackLayer = Constants.Layer.PLAYER_ATTACK,
            };

            _playerUnitData[type] = unitData;
            
            _logger.Log($"Added new player unit data for {type}.");
        }
        
        private async UniTask AddUnitBuilderData(UnitType type, WarriorConfig config)
        {
            string iconKey = config.GetUnitIconKeyForRace(RaceState.CurrentRace);
            
            var unitIcon = await _assetProvider.Load<Sprite>(iconKey);
            
            RegisterKey(iconKey);
            
            var newData = new UnitBuilderBtnData
            {
                Type = type,
                Food = GetCurrentFoodIcon,
                UnitIcon = unitIcon,
                FoodPrice = config.FoodPrice,
            };

            _unitBuilderData[(int)type] = newData;
                
            _logger.Log($"Added new unit builder data for {type}.");
        }

        private void RegisterKey(string key)
        {
            _loadedResources.Add(key);
        }
        
        private void UnloadResources()
        {
            foreach (var key in _loadedResources)
            {
                _assetProvider.Release(key);
            }
        }

        private void ClearKeyCache()
        {
            _loadedResources.Clear();
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