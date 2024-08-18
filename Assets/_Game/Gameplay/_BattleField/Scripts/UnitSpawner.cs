using System;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Units.Factory;
using Assets._Game.Common;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class UnitSpawner : IUnitSpawner, IUnitDeathObserver
    {
        public event Action<Faction, UnitType> UnitSpawned;
        public event Action<Faction, UnitType> UnitDead;

        private readonly IUnitFactory _unitFactory;
        private readonly IInteractionCache _cache;
        private readonly IWorldCameraService _cameraService;
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


        public UnitSpawner(
            IUnitFactory unitFactory, 
            IInteractionCache cache,
            IWorldCameraService cameraService,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            ICoinSpawner coinSpawner,
            IBattleSpeedManager speedManager)
        {
            _unitFactory = unitFactory;
            _cache = cache;
            _cameraService = cameraService;
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

        public void SetSpeedFactor(float speedFactor)
        {
            _enemyUnits.SetBattleSpeedFactor(speedFactor);
            _playerUnits.SetBattleSpeedFactor(speedFactor);
        }

        public void SetPaused(bool isPaused)
        {
            _playerUnits.SetPaused(isPaused);
            _enemyUnits.SetPaused(isPaused);
        }

        async void IUnitSpawner.SpawnEnemy(UnitType type)
        {
            var enemy = await _unitFactory.GetAsync(Faction.Enemy, type);
            //var enemy = _unitFactory.Get(Faction.Enemy, type);
            
            enemy.Initialize(
                _cache,
                _enemySpawnPoint,
                _enemyDestinationPoint,
                _shootProxy,
                _vfxProxy,
                _coinSpawner,
                _speedManager.CurrentSpeedFactor,
                this);
            
            _enemyUnits.Add(enemy);

            UnitSpawned?.Invoke(Faction.Enemy, type);
        }

        async void IUnitSpawner.SpawnPlayerUnit(UnitType type)
        {
            //var unit = _unitFactory.Get(Faction.Player, type);
            var unit = await _unitFactory.GetAsync(Faction.Player, type);

            unit.Initialize(
                _cache,
                _playerSpawnPoint,
                _playerDestinationPoint,
                _shootProxy,
                _vfxProxy,
                _coinSpawner,
                _speedManager.CurrentSpeedFactor,
                this);
            
            _playerUnits.Add(unit);
            
            UnitSpawned?.Invoke(Faction.Player, type);
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

        public void ResetUnits()
        {
            _playerUnits.Reset();
            _enemyUnits.Reset();
        }

        private void CalculateUnitSpawnPoints()
        {
            float correction = 0.025f;
            
            float offsetY = _cameraService.CameraHeight * correction;
            float offsetX = 0f;
            
            _playerSpawnPoint = new Vector3(-_cameraService.CameraWidth - offsetX, -offsetY, 0);
            _enemySpawnPoint = new Vector3(_cameraService.CameraWidth + offsetX , -offsetY, 0);
        }

        public void Notify(Faction faction, UnitType type)
        {
            UnitDead?.Invoke(faction, type);
        }
    }
}