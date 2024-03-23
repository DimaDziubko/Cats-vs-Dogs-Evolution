using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Battle
{
    public class BattleStateService : IBattleStateService
    {
        public event Action<BattleNavigationModel> NavigationUpdated;
        public event Action<BattleData> BattlePrepared;
        private event Action BattleChange;

        private readonly IGameConfigController _gameConfigController;
        private readonly IPersistentDataService _persistentData;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private int? _currentBattleIndex;
        private bool IsBattlePrepared { get; set; }

        private readonly HashSet<string> _loadedResources = new HashSet<string>();
        
        private readonly BattleData _currentBattleData = new BattleData();

        private readonly Dictionary<UnitType, UnitData> _enemyData = new Dictionary<UnitType, UnitData>(3);
        
        private readonly Dictionary<WeaponType, WeaponData> _weaponData = new Dictionary<WeaponType, WeaponData>(3);
        
        private Dictionary<UnitType, UnitData> _playerUnitData;
        
        private List<UnitBuilderBtnData> _unitBuilderData;
        
        private int _playerDataPreparedForAge;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public BaseData ForEnemyBase()
        {
            return _currentBattleData.EnemyBaseData;
        }

        public async UniTask Init()
        {
            IsBattlePrepared = false;
            BattleChange += OnBattleChange;

            TimelineState.NextBattleOpened += MoveToNextBattle;
            
            await PrepareBattle();
        }

        public void OnStartBattleWindowOpened()
        {
            NavigationUpdated?.Invoke(NavigationModel);
        }

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

        public BattleData BattleData
        {
            get => _currentBattleData;
        }

        public UnitData GetEnemy(UnitType type)
        {
            if (_enemyData.TryGetValue(type, out var unitData))
            {
                return unitData;
            }
            else
            {
                _logger.Log("Enemy data is empty");
                return null;
            }
        }

        private BattleNavigationModel NavigationModel =>
            new BattleNavigationModel()
            {
                IsFirstBattle = IsFirstBattle(),
                IsLastBattle = IsLastBattle(),
                CanMoveToNextBattle = CanMoveToNextBattle(),
                CanMoveToPreviousBattle = CanMoveToPreviousBattle(),
                IsPrepared = IsBattlePrepared
            };

        public BattleStateService(
            IGameConfigController gameConfigController,
            IPersistentDataService persistentData,
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _gameConfigController = gameConfigController;
            _persistentData = persistentData;
            _assetProvider = assetProvider;
            _logger = logger;
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
                BattleChange?.Invoke();
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
                UnloadResources();
                ClearKeyCache();
                
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

        private async void OnBattleChange()
        {
            _logger.Log("Battle change service");
            IsBattlePrepared = false;
            await PrepareBattle();
        }
        
        private async UniTask PrepareBattleData(CancellationToken ct)
        {
            BattleConfig battleConfig = _gameConfigController.GetBattleConfig(CurrentBattleIndex);

            ct.ThrowIfCancellationRequested();
            
            var enemyBasePrefab = await _assetProvider.Load<GameObject>(battleConfig.EnemyBaseKey);
            var bGM = await _assetProvider.Load<AudioClip>(battleConfig.BGMKey);
            
            RegisterKey(battleConfig.EnemyBaseKey);
            RegisterKey(battleConfig.BGMKey);

            _currentBattleData.Battle = CurrentBattleIndex;
            _currentBattleData.Scenario = battleConfig.Scenario;
            _currentBattleData.EnvironmentKey = battleConfig.EnvironmentKey;
            _currentBattleData.BGM = bGM;
            
            _currentBattleData.EnemyBaseData = new BaseData()
            {
                Health = battleConfig.EnemyBaseHealth,
                BasePrefab = enemyBasePrefab.GetComponent<Base>(),
                CoinsAmount = battleConfig.CoinsPerBase
            };

            _logger.Log("BattleData prepared");
        }

        private async UniTask PrepareEnemy(CancellationToken ct)
        {
            List<WarriorConfig> enemyConfigs = _gameConfigController.GetEnemyConfigs(CurrentBattleIndex);

            await PrepareEnemyData(ct, enemyConfigs);
            await PrepareWeaponData(ct, enemyConfigs);
        }

        private async UniTask PrepareWeaponData(CancellationToken ct, List<WarriorConfig> enemyConfigs)
        {
            _weaponData.Clear();
    
            foreach (var config in enemyConfigs)
            {
                ct.ThrowIfCancellationRequested();

                if(config.WeaponConfig.WeaponType == WeaponType.Melee) continue;

                Projectile projectilePrefab = null;
                MuzzleFlash muzzlePrefab = null;
                ProjectileExplosion projectileExplosionPrefab = null;

                if (config.WeaponConfig.ProjectileKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    var projectileGameObject = await _assetProvider.Load<GameObject>(config.WeaponConfig.ProjectileKey);
                    if (projectileGameObject != null) 
                    {
                        projectilePrefab = projectileGameObject.GetComponent<Projectile>();
                        RegisterKey(config.WeaponConfig.ProjectileKey);
                    }
                }
        
                if (config.WeaponConfig.MuzzleKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    var muzzleGameObject = await _assetProvider.Load<GameObject>(config.WeaponConfig.MuzzleKey);
                    if (muzzleGameObject != null) 
                    {
                        muzzlePrefab = muzzleGameObject.GetComponent<MuzzleFlash>();
                        RegisterKey(config.WeaponConfig.MuzzleKey);
                    }
                }
        
                if (config.WeaponConfig.ProjectileExplosionKey != Constants.ConfigKeys.MISSING_KEY)
                {
                    var projectileExplosionGameObject = await _assetProvider.Load<GameObject>(config.WeaponConfig.ProjectileExplosionKey);
                    if (projectileExplosionGameObject != null) 
                    {
                        projectileExplosionPrefab = projectileExplosionGameObject.GetComponent<ProjectileExplosion>();
                        RegisterKey(config.WeaponConfig.ProjectileExplosionKey);
                    }
                }

                var newData = new WeaponData()
                {
                    Config = config.WeaponConfig,
                    Layer = Constants.Layer.ENEMY_PROJECTILE,
                    ProjectilePrefab = projectilePrefab,
                    MuzzlePrefab = muzzlePrefab,
                    ProjectileExplosionPrefab = projectileExplosionPrefab,
                };

                _weaponData[config.WeaponConfig.WeaponType] = newData;
            }

            _logger.Log($"WeaponData prepared {_weaponData.Count}");
        }

        private async UniTask PrepareEnemyData(CancellationToken ct, List<WarriorConfig> enemyConfigs)
        {
            _enemyData.Clear();

            foreach (var config in enemyConfigs)
            {
                ct.ThrowIfCancellationRequested();

                var go = await _assetProvider.Load<GameObject>(config.EnemyKey);

                RegisterKey(config.EnemyKey);
                
                var newData = new UnitData
                {
                    Config = config,
                    Prefab = go.GetComponent<Unit>()
                };

                _enemyData[config.Type] = newData;
            }

            _logger.Log($"EnemyData prepared {_enemyData.Count}");
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
        
        private bool IsFirstBattle() => CurrentBattleIndex != 0;

        private bool IsLastBattle() => CurrentBattleIndex == _gameConfigController.LastBattle();

        private bool CanMoveToNextBattle() => CurrentBattleIndex < TimelineState.MaxBattle;

        private bool CanMoveToPreviousBattle() => CurrentBattleIndex > 0;

        private void CancelAndRestartCts()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}