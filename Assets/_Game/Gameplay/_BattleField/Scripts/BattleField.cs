using System;
using System.Collections.Generic;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.CoinCounter.Scripts;
using _Game.Gameplay.Vfx.Factory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BattleField : MonoBehaviour, IBaseDestructionHandler
    {
        public IUnitSpawner UnitSpawner => _unitSpawner;
        
        [SerializeField] private RectTransform _coinCounterViewTransform; 
        
        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;

        private Vector3 _enemySpawnPoint;
        private Vector3 _playerSpawnPoint;
        
        private IWorldCameraService _cameraService;
        private IAgeStateService _ageState;
        private IRandomService _random;
        private IAudioService _audioService;
        private IBaseDestructionManager _baseDestructionManager;

        private CoinSpawner _coinSpawner;
        private VfxSpawner _vfxSpawner;
        private UnitSpawner _unitSpawner;
        private ProjectileSpawner _projectileSpawner;
        private BaseSpawner _baseSpawner;

        private readonly InteractionCache _interactionCache = new InteractionCache();


        //TODO Delete later
        [ShowInInspector] public Dictionary<Collider2D, ITarget> Cache => _interactionCache.Cache;

        public void Construct(
            IUnitFactory unitFactory,
            IBaseFactory baseFactory,
            IProjectileFactory projectileFactory,
            IVfxFactory vfxFactory,
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAgeStateService ageState,
            IAudioService audioService,
            ICoinFactory coinFactory,
            IBaseDestructionManager baseDestructionManager,
            ICoinCounter coinCounter)
        {
            _cameraService = cameraService;
            _ageState = ageState;
            _audioService = audioService;
            
            _coinSpawner = new CoinSpawner(
                coinFactory, 
                audioService, 
                cameraService, 
                coinCounter);
            
            _vfxSpawner = new VfxSpawner(vfxFactory);
            
            _projectileSpawner = new ProjectileSpawner(
                projectileFactory, 
                pauseManager, 
                _interactionCache,
                _vfxSpawner);
            
            _unitSpawner = new UnitSpawner(
                unitFactory, 
                _interactionCache, 
                cameraService, 
                pauseManager, 
                _vfxSpawner, 
                _projectileSpawner, 
                _coinSpawner);
            
            _baseSpawner = new BaseSpawner(
                baseFactory,
                cameraService,
                _coinSpawner,
                baseDestructionManager,
                _interactionCache);


            _baseDestructionManager = baseDestructionManager;
            
            baseDestructionManager.Register(this);
        }

        public void Init()
        {
            CalculateBasePoints();
            CalculateUnitSpawnPoints();
            
            _unitSpawner.Init(
                _enemyBasePoint, 
                _playerBasePoint);
            
            _coinSpawner.Init(_coinCounterViewTransform);
            _baseSpawner.Init();
            
            _ageState.BaseDataUpdated += OnPlayerBaseDataUpdated;
            _ageState.AgeUpdated += OnAgeUpdated;

            _baseSpawner.UpdatePlayerBase();
        }

        private void OnAgeUpdated()
        {
            _baseSpawner.UpdatePlayerBase();
        }

        private void OnPlayerBaseDataUpdated(BaseData data) => _baseSpawner.UpdateData(data);

        public void ResetSelf() => UpdateBase(Faction.Player);

        public void StartBattle()
        {
            _baseSpawner.OnStartBattle();
        }

        public void GameUpdate()
        {
            _vfxSpawner.GameUpdate();
            _unitSpawner.GameUpdate();
            _projectileSpawner.GameUpdate();
        }

        public void UpdateBase(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    _baseSpawner.UpdatePlayerBase();
                    break;
                case Faction.Enemy:
                    _baseSpawner.UpdateEnemyBase();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
        
        public void Cleanup()
        {
            _vfxSpawner.Cleanup();
            _unitSpawner.Cleanup();
            _projectileSpawner.Cleanup();
            _interactionCache.Cleanup();
        }
        
        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            _vfxSpawner.SpawnBasesSmoke(@base.Position);
            _audioService.PlayBaseDestructionSFX();
            _unitSpawner.KillUnits(faction);
        }

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base) {}

        private void CalculateBasePoints()
        {
            _enemyBasePoint = new Vector3(_cameraService.CameraWidth, 0, 0);
            _playerBasePoint = new Vector3(-_cameraService.CameraWidth, 0, 0);
        }

        private void CalculateUnitSpawnPoints()
        {
            float correction = 0.025f;
            
            float offsetY = _cameraService.CameraHeight * correction;
            
            _playerSpawnPoint = new Vector3(-_cameraService.CameraWidth, -offsetY, 0);
            _enemySpawnPoint = new Vector3(_cameraService.CameraWidth, -offsetY, 0);
        }

        private void OnDestroy()
        {
            _ageState.BaseDataUpdated -= OnPlayerBaseDataUpdated;
            _ageState.AgeUpdated -= OnAgeUpdated;
            _baseDestructionManager.UnRegister(this);
        }
    }
}
