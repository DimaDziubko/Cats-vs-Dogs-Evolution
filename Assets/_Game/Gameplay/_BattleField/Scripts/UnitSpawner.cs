using System;
using _Game.Common;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class UnitSpawner : IUnitSpawner, IPauseHandler
    {
        private readonly IUnitFactory _unitFactory;
        private readonly IInteractionCache _cache;
        private readonly IWorldCameraService _cameraService;
        private readonly IPauseManager _pauseManager;
        private readonly IVFXProxy _vfxProxy;
        private readonly IShootProxy _shootProxy;
        private readonly ICoinSpawner _coinSpawner;

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
            IPauseManager pauseManager,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            ICoinSpawner coinSpawner)
        {
            _unitFactory = unitFactory;
            _cache = cache;
            _cameraService = cameraService;
            _pauseManager = pauseManager;
            _vfxProxy = vfxProxy;
            _coinSpawner = coinSpawner;
            _shootProxy = shootProxy;
        }
        
        public void Init(
            Vector3 playerDestination, 
            Vector3 enemyDestination)
        {
            _playerDestinationPoint = playerDestination;
            _enemyDestinationPoint = enemyDestination;
            
            CalculateUnitSpawnPoints();
            _pauseManager.Register(this);
        }

        public void Cleanup()
        {
            _enemyUnits.Clear();
            _playerUnits.Clear();
        }

        public void GameUpdate()
        {
            _playerUnits.GameUpdate();
            _enemyUnits.GameUpdate();
        }

        public void SetPaused(bool isPaused)
        {
            _playerUnits.SetPaused(isPaused);
            _enemyUnits.SetPaused(isPaused);
        }

        void IUnitSpawner.SpawnEnemy(UnitType type)
        {
            var enemy = _unitFactory.Get(Faction.Enemy, type);
            
            enemy.Initialize(
                _cache,
                _enemySpawnPoint,
                _enemyDestinationPoint,
                _shootProxy,
                _vfxProxy,
                _coinSpawner);
            
            _enemyUnits.Add(enemy);
        }

        void IUnitSpawner.SpawnPlayerUnit(UnitType type)
        {
            //TODO Delete
            Debug.Log($"{_playerSpawnPoint} SPAWNER");
            
            var unit = _unitFactory.Get(Faction.Player, type);

            unit.Initialize(
                _cache,
                _playerSpawnPoint,
                _playerDestinationPoint,
                _shootProxy,
                _vfxProxy,
                _coinSpawner);
            
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
            
            _playerSpawnPoint = new Vector3(-_cameraService.CameraWidth, -offsetY, 0);
            _enemySpawnPoint = new Vector3(_cameraService.CameraWidth, -offsetY, 0);
        }
    }
}