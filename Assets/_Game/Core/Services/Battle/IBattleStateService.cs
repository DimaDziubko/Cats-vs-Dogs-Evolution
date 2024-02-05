using System;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.Gameplay.Battle.Scripts;

namespace _Game.Core.Services.Battle
{
    public interface IBattleStateService
    {
        event Action BattleChanged;
        int CurrentBattleIndex { get; }
        bool IsFirstBattle();
        bool IsLastBattle();
        bool CanMoveToNextBattle();
        bool CanMoveToPreviousBattle();
        void MoveToNextBattle();
        void MoveToPreviousBattle();
        BattleConfig GetCurrentBattleConfig();
        BattleAsset GetBattleAsset();
        BattleScenario GetScenario();
        WarriorConfig GetEnemyConfig(in int index);
    }

    public class BattleStateService : IBattleStateService
    {
        public event Action BattleChanged;
        
        private readonly IGameConfigController _gameConfigController;
        private readonly IPersistentDataService _persistentDataService;
        private readonly IAssetProvider _assetProvider;
        private IUserTimelineStateReadonly TimelineState => _persistentDataService.State.TimelineState;

        private int? _currentBattleIndex;

        public BattleStateService(
            IGameConfigController gameConfigController,
            IPersistentDataService persistentDataService,
            IAssetProvider assetProvider)
        {
            _gameConfigController = gameConfigController;
            _persistentDataService = persistentDataService;
            _assetProvider = assetProvider;

            BattleChanged += OnBattleChanged;
        }

        public int CurrentBattleIndex
        {
            get
            {
                if (_currentBattleIndex == null)
                {
                    _currentBattleIndex = TimelineState.MaxBattle;
                    BattleChanged?.Invoke();
                }
                return _currentBattleIndex.Value;
            }
            private set
            {
                _currentBattleIndex = value;
                BattleChanged?.Invoke();
            }
        }

        public bool IsFirstBattle() => CurrentBattleIndex != 0;

        public bool IsLastBattle() => CurrentBattleIndex == _gameConfigController.LastBattleIndex();

        public bool CanMoveToNextBattle() => CurrentBattleIndex < TimelineState.MaxBattle;

        public bool CanMoveToPreviousBattle() => CurrentBattleIndex > 0;

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

        public BattleConfig GetCurrentBattleConfig() => _gameConfigController.GetBattleConfig(CurrentBattleIndex);

        public BattleAsset GetBattleAsset() => _assetProvider.ForBattle(CurrentBattleIndex);

        public BattleScenario GetScenario() => _gameConfigController.GetBattleScenario(CurrentBattleIndex);
        public WarriorConfig GetEnemyConfig(in int index) => _gameConfigController.GetEnemyConfig(CurrentBattleIndex, index);

        private void OnBattleChanged()
        {
            var enemyKey = _gameConfigController.GetEnemiesKey(CurrentBattleIndex);
            _assetProvider.LoadEnemiesAsync(enemyKey);
        }
    }
}