using System;
using _Game.Common;
using _Game.Core._GameListenerComposite;
using _Game.Core.Services.Camera;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Creatives.Scripts
{
    public class CrUnitSpawner : IUnitSpawner, IPauseHandler, IBattleSpeedListener, IUnitDeathObserver
    {
        public event Action<Faction, UnitType> UnitDead;
        public event Action<Faction, UnitType> UnitSpawned;

        private readonly IUnitFactory _unitFactory;
        private readonly IInteractionCache _cache;
        private readonly IWorldCameraService _cameraService;
        private readonly IPauseManager _pauseManager;
        private readonly IVFXProxy _vfxProxy;
        private readonly IShootProxy _shootProxy;
        private readonly ICoinSpawner _coinSpawner;
        private readonly IBattleSpeedManager _speedManager;

        private readonly GameBehaviourCollection _playerUnits = new GameBehaviourCollection();
        private readonly GameBehaviourCollection _enemyUnits = new GameBehaviourCollection();

        private Vector3 _playerDestinationPoint;
        private Vector3 _enemyDestinationPoint;

        private Vector3 _playerSpawnPoint;
        private Vector3 _enemySpawnPoint;


        public CrUnitSpawner(
            IUnitFactory unitFactory, 
            IInteractionCache cache,
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            ICoinSpawner coinSpawner,
            IBattleSpeedManager speedManager)
        {
            _unitFactory = unitFactory;
            _cache = cache;
            _cameraService = cameraService;
            _pauseManager = pauseManager;
            _vfxProxy = vfxProxy;
            _coinSpawner = coinSpawner;
            _shootProxy = shootProxy;
            _speedManager = speedManager;
        }

        public void Init(
            Vector3 playerDestination, 
            Vector3 enemyDestination)
        {
            _playerDestinationPoint = playerDestination;
            _enemyDestinationPoint = enemyDestination;
            
            CalculateUnitSpawnPoints();
            _pauseManager.AddHandler(this);
            _speedManager.Register(this);
        }

        public void Cleanup()
        {
            _enemyUnits.Clear();
            _playerUnits.Clear();
        }

        public void GameUpdate(float deltaTime)
        {
            _playerUnits.GameUpdate(deltaTime);
            _enemyUnits.GameUpdate(deltaTime);
        }


        void IPauseHandler.SetPaused(bool isPaused)
        {
            _playerUnits.SetPaused(isPaused);
            _enemyUnits.SetPaused(isPaused);
        }

        void IUnitSpawner.SpawnEnemy(UnitType type)
        {
            var enemy = _unitFactory.Get(Faction.Enemy, type);

            Vector3 enemyDestination;
            
            enemy.Initialize(
                _cache,
                _enemySpawnPoint,
                CrQuickGame.I.GetRandomEnemyDestination(),
                _shootProxy,
                _vfxProxy,
                _coinSpawner,
                _speedManager.CurrentSpeedFactor,
                this);

            if (CrQuickGame.I.EnemySpawnPoints.Length > 0)
            {
                enemy.Position = CrQuickGame.I.GetRandomEnemySpawnPoint();
            }
            
            _enemyUnits.Add(enemy);
        }

        void IUnitSpawner.SpawnPlayerUnit(UnitType type)
        {
            var unit = _unitFactory.Get(Faction.Player, type);

            unit.Initialize(
                _cache,
                _playerSpawnPoint,
                CrQuickGame.I.GetRandomPlayerDestination(),
                _shootProxy,
                _vfxProxy,
                _coinSpawner,
                _speedManager.CurrentSpeedFactor,
                this);

            if (CrQuickGame.I.PlayerSpawnPoints.Length > 0)
            {
                unit.Position = CrQuickGame.I.GetRandomPlayerSpawnPoint();
            }
            
            _playerUnits.Add(unit);
        }

        public void KillUnits(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    _playerUnits.Clear();
                    break;
                case Faction.Enemy:
                    _enemyUnits.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        private void CalculateUnitSpawnPoints()
        {
            float correction = 0.025f;
            
            float offsetY = _cameraService.CameraHeight * correction;
            float offsetX = 0f;
            
            _playerSpawnPoint = new Vector3(-_cameraService.CameraWidth - offsetX, -offsetY, 0);
            _enemySpawnPoint = new Vector3(_cameraService.CameraWidth + offsetX , -offsetY, 0);
        }

        void IBattleSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            _enemyUnits.SetBattleSpeedFactor(speedFactor);
            _playerUnits.SetBattleSpeedFactor(speedFactor);
        }

        public void Notify(Faction faction, UnitType type)
        {
            UnitDead?.Invoke(faction, type);
        }
    }
}
