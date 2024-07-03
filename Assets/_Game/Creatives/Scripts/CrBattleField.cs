using Assets._Game.Core.Factory;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.UI._Hud;
using UnityEngine;

namespace Assets._Game.Creatives.Scripts
{
    public class CrBattleField
    {
        public IUnitSpawner UnitSpawner => _unitSpawner;
        
        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;

        //TODO Use later
        private Vector3 _enemySpawnPoint;
        private Vector3 _playerSpawnPoint;
        
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private readonly CoinSpawner _coinSpawner;
        private readonly VfxSpawner _vfxSpawner;
        private readonly CrUnitSpawner _unitSpawner;
        private readonly ProjectileSpawner _projectileSpawner;

        private readonly CoinCounterView _coinCounterView;

        private readonly InteractionCache _interactionCache = new InteractionCache();
        
        public CrBattleField(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAudioService audioService,
            ICoinCounter coinCounter,
            IFactoriesHolder factoriesHolder,
            IBattleSpeedManager speedManager,
            CrHud hud)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            
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
            
            _unitSpawner = new CrUnitSpawner(
                factoriesHolder.UnitFactory, 
                _interactionCache, 
                cameraService, 
                pauseManager, 
                _vfxSpawner, 
                _projectileSpawner, 
                _coinSpawner,
                speedManager);
            
            _coinCounterView = hud.CounterView;
        }

        public void Init()
        {
            CalculateBasePoints();
            CalculateUnitSpawnPoints();
            
            _unitSpawner.Init(
                _enemyBasePoint, 
                _playerBasePoint);
            
            _coinSpawner.Init(_coinCounterView.CoinIconHolderPosition);
        }
        
        public void GameUpdate()
        {
            _vfxSpawner.GameUpdate();
            _unitSpawner.GameUpdate();
            _projectileSpawner.GameUpdate();
        }
        
        
        public void Cleanup()
        {
            _vfxSpawner.Cleanup();
            _unitSpawner.Cleanup();
            _projectileSpawner.Cleanup();
            _interactionCache.Cleanup();
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