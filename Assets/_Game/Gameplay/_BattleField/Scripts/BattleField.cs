using System;
using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.UI._Hud;
using _Game.UI._Hud._CoinCounterView;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BattleField : IBaseDestructionHandler
    {
        public IUnitSpawner UnitSpawner => _unitSpawner;
        
        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;

        //TODO Use later
        private Vector3 _enemySpawnPoint;
        private Vector3 _playerSpawnPoint;
        
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;

        private readonly CoinSpawner _coinSpawner;
        private readonly VfxSpawner _vfxSpawner;
        private readonly UnitSpawner _unitSpawner;
        private readonly ProjectileSpawner _projectileSpawner;
        private readonly BaseSpawner _baseSpawner;

        private readonly CoinCounterView _coinCounterView;

        private readonly InteractionCache _interactionCache = new InteractionCache();

        public BattleField(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAudioService audioService,
            IBaseDestructionManager baseDestructionManager,
            ICoinCounter coinCounter,
            IFactoriesHolder factoriesHolder,
            IBattleSpeedManager speedManager,
            Hud hud,
            IBasePresenter basePresenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _pauseManager = pauseManager;
            
            _coinSpawner = new CoinSpawner(
                factoriesHolder.CoinFactory, 
                audioService,
                coinCounter);
            
            _vfxSpawner = new VfxSpawner(factoriesHolder.VfxFactory);
            
            _projectileSpawner = new ProjectileSpawner(
                factoriesHolder.ProjectileFactory, 
                pauseManager, 
                _interactionCache,
                _vfxSpawner,
                speedManager);
            
            _unitSpawner = new UnitSpawner(
                factoriesHolder.UnitFactory, 
                _interactionCache, 
                cameraService, 
                pauseManager, 
                _vfxSpawner, 
                _projectileSpawner, 
                _coinSpawner,
                speedManager);
            
            _baseSpawner = new BaseSpawner(
                factoriesHolder.BaseFactory,
                cameraService,
                _coinSpawner,
                baseDestructionManager,
                _interactionCache,
                basePresenter);


            _coinCounterView = hud.CounterView;
            
            baseDestructionManager.Register(this);
        }

        public void Init()
        {
            CalculateBasePoints();
            CalculateUnitSpawnPoints();
            
            _unitSpawner.Init(
                _enemyBasePoint, 
                _playerBasePoint);
            
            _coinSpawner.Init(_coinCounterView.CoinIconHolderPosition);
            _baseSpawner.Init();
        }
        
        public void ResetSelf() => UpdateBase(Faction.Player);

        public void StartBattle() => _baseSpawner.OnStartBattle();

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
            _baseSpawner.Cleanup();
        }
        
        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            _vfxSpawner.SpawnBasesSmoke(@base.Position);
            _audioService.PlayBaseDestructionSFX();
            _unitSpawner.ResetUnits();
            _unitSpawner.KillUnits(faction);
        }

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            _pauseManager.SetPaused(true);
        }

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
        
    }
}
