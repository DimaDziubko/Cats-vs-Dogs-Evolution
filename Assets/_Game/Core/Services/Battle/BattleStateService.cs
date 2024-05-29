using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
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
    public class BattleStateService : IBattleStateService, IDisposable
    {
        public event Action<BattleNavigationModel> NavigationUpdated;
        public event Action<BattleData> BattlePrepared;
        private event Action BattleChanged;

        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IPersistentDataService _persistentData;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        private readonly IUnitDataProvider _unitDataProvider;
        private readonly IWeaponDataProvider _weaponDataProvider;
        private readonly IBaseDataProvider _baseDataProvider;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;

        private int? _currentBattleIndex;

        private readonly Dictionary<int, BattleData> _battlesData = new Dictionary<int, BattleData>(6);
        private readonly Dictionary<int, Dictionary<UnitType, UnitData>> _unitsData = new Dictionary<int, Dictionary<UnitType, UnitData>>(6);
        private readonly Dictionary<int,  Dictionary<WeaponType, WeaponData>> _weaponsData = new Dictionary<int,  Dictionary<WeaponType, WeaponData>>(6);

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public BattleData GetCurrentBattleData()
        {
            if (_battlesData.TryGetValue(CurrentBattleIndex, out var battleData))
            {
                return battleData;
            }
            else
            {
                _logger.LogError("Battle data not found for index: " + CurrentBattleIndex);
                return null;
            }
        }

        public BaseData ForEnemyBase() => 
            GetCurrentBattleData().EnemyBaseData;

        public BattleStateService(
            ITimelineConfigRepository timelineConfigRepository,
            IPersistentDataService persistentData,
            IAssetRegistry assetRegistry,
            IMyLogger logger,
            IUnitDataProvider unitDataProvider,
            IWeaponDataProvider weaponDataProvider,
            IBaseDataProvider baseDataProvider)
        {
            _timelineConfigRepository = timelineConfigRepository;
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

        public async UniTask Init()
        {
            BattleChanged += OnBattleChanged;
            TimelineState.NextBattleOpened += MoveToNextBattle;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            
            await PrepareBattles();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            BattleChanged -= OnBattleChanged;
            TimelineState.NextBattleOpened -= MoveToNextBattle;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
        }

        public async UniTask ChangeRace() => 
            await PrepareBattles();

        public void ReleaseResources() => _assetRegistry.ClearContext(Constants.CacheContext.BATTLE);

        public void OnStartBattleWindowOpened() => 
            NavigationUpdated?.Invoke(NavigationModel);

        public WeaponData ForWeapon(WeaponType type)
        {
            if (_weaponsData[CurrentBattleIndex].TryGetValue(type, out var projectileData))
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

            if (IsLastBattle()) _persistentData.SetAllBattlesWon(true);

            if (!IsLastBattle() && !CanMoveToNextBattle()) _persistentData.OpenNextBattle(nextBattle);
        }

        public UnitData GetEnemy(UnitType type)
        {
            if (_unitsData[CurrentBattleIndex].TryGetValue(type, out var unitData))
            {
                return unitData;
            }
            else
            {
                _logger.Log("Enemy data is empty");
                return null;
            }
        }

        public void MoveToNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                CurrentBattleIndex++;
            }
        }

        public void MoveToPreviousBattle()
        {
            if (CanMoveToPreviousBattle())
            {
                CurrentBattleIndex--;
            }
        }

        private void OnNextAgeOpened() => 
            CurrentBattleIndex = TimelineState.MaxBattle;

        private BattleNavigationModel NavigationModel =>
            new BattleNavigationModel()
            {
                CanMoveToNextBattle = CanMoveToNextBattle(),
                CanMoveToPreviousBattle = CanMoveToPreviousBattle(),
                IsPrepared = true
            };

        private async UniTask PrepareBattles()
        {
            var ct = _cts.Token;
            try
            {
                ReleaseResources();
                IEnumerable<BattleConfig> configs = _timelineConfigRepository.GetBattleConfigs();
                var prepareBattleDataTask = PrepareBattlesData(configs, ct);
                var prepareEnemyDataTask = PrepareEnemies(configs, ct);

                await UniTask.WhenAll(prepareBattleDataTask, prepareEnemyDataTask);
                
                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private void OnBattleChanged()
        {
            _logger.Log($"Battle change service {CurrentBattleIndex}");
            //TODO Change
            //await PrepareBattles();
            BattlePrepared?.Invoke(_battlesData[CurrentBattleIndex]);
            NavigationUpdated?.Invoke(NavigationModel);
        }

        private async UniTask PrepareBattlesData(IEnumerable<BattleConfig> configs, CancellationToken ct)
        {
            foreach (var config in configs)
            {
                await PrepareBattleData(config, ct);
            }
            
            _logger.Log($"Battles prepared count {_battlesData.Count}");
        }

        private async UniTask PrepareBattleData(BattleConfig config, CancellationToken ct)
        {
            BattleData data = new BattleData();
            
            string key = config.GetBaseKeyForAnotherRace(RaceState.CurrentRace);
            var baseLoadOptions = new BaseLoadOptions()
            {
                Faction = Faction.Enemy,
                CacheContext = Constants.CacheContext.BATTLE,
                PrefabKey = key,
                Health = config.EnemyBaseHealth,
                CancellationToken = ct,
                CoinsAmount = config.CoinsPerBase,
            };
            
            data.Battle = config.Id;
            data.Scenario = config.Scenario;
            data.BGM = await _assetRegistry.LoadAsset<AudioClip>(config.BGMKey, Constants.CacheContext.BATTLE);
            data.EnemyBaseData = await _baseDataProvider.LoadBase(baseLoadOptions);
            data.MaxCoinsPerBattle = config.MaxCoinsPerBattle;
            
            GameObject environmentGO =
                await _assetRegistry.LoadAsset<GameObject>(config.EnvironmentKey, Constants.CacheContext.GENERAL);

            environmentGO.TryGetComponent(out BattleEnvironment prefab);
            
            data.EnvironmentData = new EnvironmentData()
            {
                Key = config.EnvironmentKey,
                Prefab = prefab,
            };
            
            data.AnalyticsData = new BattleAnalyticsData()
            {
                TimelineNumber = TimelineState.TimelineId + 1,
                AgeNumber = TimelineState.AgeId + 1,
                BattleNumber = CurrentBattleIndex + 1
            };
            
            _battlesData.Add(config.Id, data);
            
            _logger.Log($"BattleData prepared Id{config.Id}");
        }

        private async UniTask PrepareEnemies(IEnumerable<BattleConfig> configs, CancellationToken ct)
        {
            foreach (var config in configs)
            {
                Dictionary<UnitType, UnitData> enemyUnitsData = new Dictionary<UnitType, UnitData>(3);
                Dictionary<WeaponType, WeaponData> weaponData = new Dictionary<WeaponType, WeaponData>(3);
                
                await PrepareEnemyUnitsData(enemyUnitsData, config.Enemies, ct);
                await PrepareWeaponsData(weaponData, config.Enemies, ct);
                
                _unitsData.Add(config.Id, enemyUnitsData);
                _weaponsData.Add(config.Id, weaponData);
            }
        }

        private async UniTask PrepareEnemyUnitsData(
            Dictionary<UnitType, UnitData> enemyUnitsData, 
            IEnumerable<WarriorConfig> enemyConfigs, 
            CancellationToken ct)
        {
            foreach (var config in enemyConfigs)
            {
                await PrepareEnemyUnitData(enemyUnitsData, config, ct);
            }
            
            _logger.Log("EnemyUnitData prepared");
        }

        private async UniTask PrepareEnemyUnitData(
            Dictionary<UnitType, UnitData> enemyUnitsData, 
            WarriorConfig config, 
            CancellationToken ct)
        {
            var unitLoadOptions = new UnitLoadOptions()
            {
                Faction = Faction.Enemy,
                Config = config,
                CacheContext = Constants.CacheContext.BATTLE,
                CurrentRace = RaceState.CurrentRace,
                CancellationToken = ct,
            };
            
            enemyUnitsData[config.Type] =
                await _unitDataProvider.LoadUnitData(unitLoadOptions);
        }

        private async UniTask PrepareWeaponsData(
            Dictionary<WeaponType, WeaponData> weaponData, 
            IEnumerable<WarriorConfig> warriorConfigs, 
            CancellationToken ct)
        {
            foreach (var config in warriorConfigs)
            {
                await PrepareWeaponData(weaponData, ct, config.WeaponConfig);
            }

            _logger.Log($"WeaponData prepared {weaponData.Count}");
        }

        private async UniTask PrepareWeaponData(
            Dictionary<WeaponType, WeaponData> weaponData, 
            CancellationToken ct, 
            WeaponConfig config)
        {
            if (config.WeaponType == WeaponType.Melee) return;

            var weaponLoadOptions = new WeaponLoadOptions()
            {
                Faction = Faction.Enemy,
                Config = config,
                CacheContext = Constants.CacheContext.BATTLE,
                CancellationToken = ct
            };
            
            weaponData[config.WeaponType] =
                await _weaponDataProvider.LoadWeapon(weaponLoadOptions);
        }

        private bool IsFirstBattle() => CurrentBattleIndex != 0;

        private bool IsLastBattle() => CurrentBattleIndex == _timelineConfigRepository.LastBattle();

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