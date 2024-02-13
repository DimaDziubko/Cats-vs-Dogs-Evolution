using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Bundles.Bases.Scripts;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Battle
{
    public class BattleStateService : IBattleStateService
    {
        public event Action<BattleNavigationModel> BattleChange;
        public event Action<BattleData> BattlePrepared;

        private readonly IGameConfigController _gameConfigController;
        private readonly IPersistentDataService _persistentDataService;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;
        private IUserTimelineStateReadonly TimelineState => _persistentDataService.State.TimelineState;

        private int? _currentBattleIndex;
        public bool IsBattlePrepared { get; private set; }

        private readonly BattleData _currentBattleData = new BattleData();

        private readonly Dictionary<UnitType, UnitData> _enemyData = new Dictionary<UnitType, UnitData>(3);
        
        private Dictionary<UnitType, UnitData> _playerUnitData;
        private List<UnitBuilderBtnData> _unitBuilderData;
        private int _playerDataPreparedForAge;
        
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        public BaseData GetForEnemyBase()
        {
            return _currentBattleData.EnemyBaseData;
        }

        public async UniTask Init()
        {
            await PrepareBattle();
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
        
        public BattleNavigationModel NavigationModel =>
            new BattleNavigationModel()
            {
                IsFirstBattle = IsFirstBattle(),
                IsLastBattle = IsLastBattle(),
                CanMoveToNextBattle = CanMoveToNextBattle(),
                CanMoveToPreviousBattle = CanMoveToPreviousBattle(),
            };

        public BattleStateService(
            IGameConfigController gameConfigController,
            IPersistentDataService persistentDataService,
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _gameConfigController = gameConfigController;
            _persistentDataService = persistentDataService;
            _assetProvider = assetProvider;
            _logger = logger;

            IsBattlePrepared = false;

            BattleChange += OnBattleChange;
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
                BattleChange?.Invoke(NavigationModel);
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
            }
        }

        private async UniTask PrepareBattle()
        {
            var ct = _cts.Token;
            try
            {
                await PrepareBattleData(ct);
                await PrepareEnemyData(ct);
                ct.ThrowIfCancellationRequested();

                SetBattlePreparationState(true);
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }
        
        private async void OnBattleChange(BattleNavigationModel model)
        {
            _logger.Log("Battle change");
            IsBattlePrepared = false;
            await PrepareBattle();
        }
        
        private async UniTask PrepareBattleData(CancellationToken ct)
        {
            BattleConfig battleConfig = _gameConfigController.GetBattleConfig(CurrentBattleIndex);

            ct.ThrowIfCancellationRequested();
            Sprite environment = await _assetProvider.Load<Sprite>(battleConfig.BackgroundKey);
            var enemyBasePrefab = await _assetProvider.Load<GameObject>(battleConfig.EnemyBaseKey);
            
            _currentBattleData.Scenario = battleConfig.Scenario;
            _currentBattleData.Environment = environment;
            _currentBattleData.EnemyBaseData = new BaseData()
            {
                Health = battleConfig.EnemyBaseHealth,
                BasePrefab = enemyBasePrefab.GetComponent<Base>()
            };

            _logger.Log("BattleData prepared");
        }

        private async UniTask PrepareEnemyData(CancellationToken ct)
        {
            List<WarriorConfig> enemyConfigs = _gameConfigController.GetEnemyConfigs(CurrentBattleIndex);

            _enemyData.Clear();

            foreach (var config in enemyConfigs)
            {
                ct.ThrowIfCancellationRequested();

                var go = await _assetProvider.Load<GameObject>(config.EnemyKey);
                var newData = new UnitData
                {
                    Config = config,
                    Prefab = go.GetComponent<Unit>()
                };

                _enemyData[config.Type] = newData;
            }

            _logger.Log($"EnemyData prepared {_enemyData.Count}");
        }

        private bool IsFirstBattle() => CurrentBattleIndex != 0;

        private bool IsLastBattle() => CurrentBattleIndex == _gameConfigController.LastBattleIndex();

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