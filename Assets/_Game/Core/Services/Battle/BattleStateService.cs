using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.DataProviders;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._Environment;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Battle
{
    public class BattleStateService : IBattleStateService
    {
        public event Action<BattleNavigationModel> NavigationUpdated;
        public event Action<BattleData> BattlePrepared;
        private event Action BattleChanged;

        private readonly IGameConfigController _gameConfigController;
        private readonly IPersistentDataService _persistentData;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly IWeaponDataProvider _weaponDataProvider;
        private readonly IBaseDataProvider _baseDataProvider;
        
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;

        private int? _currentBattleIndex;
        private bool IsBattlePrepared { get; set; }

        private readonly BattleData _currentBattleData = new BattleData();

        private readonly Dictionary<UnitType, UnitData> _enemyUnitData = new Dictionary<UnitType, UnitData>(3);
        
        private readonly Dictionary<WeaponType, WeaponData> _weaponData = new Dictionary<WeaponType, WeaponData>(3);
        
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public BaseData ForEnemyBase() => 
            _currentBattleData.EnemyBaseData;

        public async UniTask Init()
        {
            IsBattlePrepared = false;
            BattleChanged += OnBattleChanged;

            TimelineState.NextBattleOpened += MoveToNextBattle;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            
            await PrepareBattle();
        }

        public async UniTask ChangeRace() => await PrepareBattle();

        public void ReleaseResources() => _assetRegistry.ClearContext(Constants.CacheContext.BATTLE);

        public void OnStartBattleWindowOpened() => 
            NavigationUpdated?.Invoke(NavigationModel);

        public WeaponData ForWeapon(WeaponType type)
        {
            if (_weaponData.TryGetValue(type, out var projectileData))
            {
                return projectileData;
            }
            else
            {
                _logger.Log("Projectile data is empty (battle)");
                return null;
            }
        }

        public void OpenNextBattle(int nextBattle)
        {
            if (CanMoveToNextBattle())
            {
                MoveToNextBattle();
                return;
            }

            if (IsLastBattle())
            {
                _persistentData.SetAllBattlesWon(true);
            }

            if (!IsLastBattle() && !CanMoveToNextBattle())
            {
                _persistentData.OpenNextBattle(nextBattle);
            }
        }

        public BattleData BattleData => _currentBattleData;

        public UnitData GetEnemy(UnitType type)
        {
            if (_enemyUnitData.TryGetValue(type, out var unitData))
            {
                return unitData;
            }
            else
            {
                _logger.Log("Enemy data is empty");
                return null;
            }
        }

        public BattleStateService(
            IGameConfigController gameConfigController,
            IPersistentDataService persistentData,
            IAssetRegistry assetRegistry,
            IMyLogger logger,
            IUnitDataProvider unitDataProvider,
            IWeaponDataProvider weaponDataProvider,
            IBaseDataProvider baseDataProvider)
        {
            _gameConfigController = gameConfigController;
            _persistentData = persistentData;
            _assetRegistry = assetRegistry;
            _logger = logger;
            _unitDataProvider = unitDataProvider;
            _weaponDataProvider = weaponDataProvider;
            _baseDataProvider = baseDataProvider;
        }

        public int CurrentBattleIndex
        {
            get
            {
                _currentBattleIndex ??= TimelineState.MaxBattle;
                return _currentBattleIndex.Value;
            }
            private set
            {
                _currentBattleIndex = value;
                BattleChanged?.Invoke();
            }
        }

        public void MoveToNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                CancelAndRestartCts();
                SetBattlePreparationState(false);
                CurrentBattleIndex++;
            }
        }

        public void MoveToPreviousBattle()
        {
            if (CanMoveToPreviousBattle())
            {
                CancelAndRestartCts();
                SetBattlePreparationState(false);
                CurrentBattleIndex--;
            }
        }

        private void OnNextAgeOpened() => 
            CurrentBattleIndex = TimelineState.MaxBattle;

        private BattleNavigationModel NavigationModel =>
            new BattleNavigationModel()
            {
                IsFirstBattle = IsFirstBattle(),
                IsLastBattle = IsLastBattle(),
                CanMoveToNextBattle = CanMoveToNextBattle(),
                CanMoveToPreviousBattle = CanMoveToPreviousBattle(),
                IsPrepared = IsBattlePrepared
            };

        private void SetBattlePreparationState(bool isPrepared)
        {
            IsBattlePrepared = isPrepared;
            
            if (isPrepared)
            {
                BattlePrepared?.Invoke(_currentBattleData);
                NavigationUpdated?.Invoke(NavigationModel);
                
                _logger.Log("Battle prepared (service).");
            }
        }

        private async UniTask PrepareBattle()
        {
            var ct = _cts.Token;
            try
            {
                ReleaseResources();
                
                var prepareBattleDataTask = PrepareBattleData(ct);
                var prepareEnemyDataTask = PrepareEnemy(ct);

                await UniTask.WhenAll(prepareBattleDataTask, prepareEnemyDataTask);
                
                ct.ThrowIfCancellationRequested();

                SetBattlePreparationState(true);
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private async void OnBattleChanged()
        {
            _logger.Log("Battle change service");
            IsBattlePrepared = false;
            await PrepareBattle();
        }
        
        private async UniTask PrepareBattleData(CancellationToken ct)
        {
            BattleConfig battleConfig = _gameConfigController.GetBattleConfig(CurrentBattleIndex);
            
            var bGm = await _assetRegistry.LoadAsset<AudioClip>(battleConfig.BGMKey, Constants.CacheContext.BATTLE);
            string key = battleConfig.GetBaseKeyForAnotherRace(RaceState.CurrentRace);

            var baseLoadOptions = new BaseLoadOptions()
            {
                Faction = Faction.Enemy,
                CacheContext = Constants.CacheContext.BATTLE,
                PrefabKey = key,
                Health = battleConfig.EnemyBaseHealth,
                CancellationToken = ct,
                CoinsAmount  = battleConfig.CoinsPerBase,
            };
            
            _currentBattleData.Battle = CurrentBattleIndex;
            _currentBattleData.Scenario = battleConfig.Scenario;
            _currentBattleData.BGM = bGm;
            _currentBattleData.EnemyBaseData = 
                await _baseDataProvider.LoadBase(baseLoadOptions);
            _currentBattleData.MaxCoinsPerBattle = battleConfig.MaxCoinsPerBattle;
            
            GameObject environmentGO =
                await _assetRegistry.LoadAsset<GameObject>(battleConfig.EnvironmentKey, Constants.CacheContext.GENERAL);

            environmentGO.TryGetComponent(out BattleEnvironment prefab);
            
            _currentBattleData.EnvironmentData = new EnvironmentData()
            {
                Key = battleConfig.EnvironmentKey,
                Prefab = prefab,
            };
            
            _logger.Log("BattleData prepared");
        }

        private async UniTask PrepareEnemy(CancellationToken ct)
        {
            List<WarriorConfig> enemyConfigs = _gameConfigController.GetEnemyConfigs(CurrentBattleIndex);

            await PrepareEnemyUnitsData(enemyConfigs, ct);
            await PrepareWeaponsData(enemyConfigs, ct);
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
        
        private async UniTask PrepareWeaponData(CancellationToken ct, WeaponConfig config)
        {
            if (config.WeaponType == WeaponType.Melee) return;

            var weaponLoadOptions = new WeaponLoadOptions()
            {
                Faction = Faction.Enemy,
                Config = config,
                CacheContext = Constants.CacheContext.BATTLE,
                CancellationToken = ct
            };
            
            _weaponData[config.WeaponType] =
                await _weaponDataProvider.LoadWeapon(weaponLoadOptions);
        }
        

        private async UniTask PrepareEnemyUnitsData(List<WarriorConfig> enemyConfigs, CancellationToken ct)
        {
            _enemyUnitData.Clear();
            
            foreach (var config in enemyConfigs)
            {
                await PrepareEnemyUnitData(config, ct);
            }
            
            _logger.Log("EnemyUnitData prepared");
            
        }

        private async UniTask PrepareEnemyUnitData(WarriorConfig config, CancellationToken ct)
        {
            var unitLoadOptions = new UnitLoadOptions()
            {
                Faction = Faction.Enemy,
                Config = config,
                CacheContext = Constants.CacheContext.BATTLE,
                CurrentRace = RaceState.CurrentRace,
                CancellationToken = ct,
            };
            
            _enemyUnitData[config.Type] =
                await _unitDataProvider.LoadUnitData(unitLoadOptions);
        }
        
        private bool IsFirstBattle() => CurrentBattleIndex != 0;

        private bool IsLastBattle() => CurrentBattleIndex == _gameConfigController.LastBattle();

        private bool CanMoveToNextBattle() => CurrentBattleIndex < TimelineState.MaxBattle;

        private bool CanMoveToPreviousBattle() => CurrentBattleIndex > 0;

        private void CancelAndRestartCts()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _cts = new CancellationTokenSource();
        }
    }
}