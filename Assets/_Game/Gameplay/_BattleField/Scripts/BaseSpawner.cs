using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BaseSpawner : IBaseSpawner
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IBaseFactory _baseFactory;
        private readonly ICoinSpawner _coinSpawner;
        private readonly IBaseDestructionManager _baseDestructionManager;
        private readonly IInteractionCache _interactionCache;
        private readonly IBasePresenter _basePresenter;

        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;

        private Base _enemyBase, _playerBase;

        public BaseSpawner(
            IBaseFactory baseFactory,
            IWorldCameraService cameraService,
            ICoinSpawner coinSpawner,
            IBaseDestructionManager baseDestructionManager,
            IInteractionCache interactionCache,
            IBasePresenter basePresenter)
        {
            _baseFactory = baseFactory;
            _cameraService = cameraService;
            _coinSpawner = coinSpawner;
            _baseDestructionManager = baseDestructionManager;
            _interactionCache = interactionCache;
            _basePresenter = basePresenter;
        }

        public void OnStartBattle()
        {
            _enemyBase.InteractionCache = _interactionCache;
            _playerBase.InteractionCache = _interactionCache;
            _enemyBase.ShowHealth();
            _playerBase.ShowHealth();
            
        }

        public void Init()
        {
            CalculateBasePoints();
            UpdatePlayerBase();

            _basePresenter.PlayerBaseDataUpdated += UpdateData;
            _basePresenter.PlayerBaseUpdated += UpdatePlayerBase;

            //_ageState.BaseDataUpdated += UpdateData;
            //_ageState.AgeUpdated += UpdatePlayerBase;
            //_ageState.RaceChangingBegun += RemoveBases;
        }

        public void UpdatePlayerBase()
        {
            if (_playerBase != null)
            {
                _playerBase.Recycle();
            }
            
            _playerBase = _baseFactory.GetBase(Faction.Player);
            _playerBase.PrepareIntro(
                _playerBasePoint, 
                _coinSpawner, 
                _baseDestructionManager);
        }

        public void UpdateEnemyBase()
        {
            if (_enemyBase != null)
            {
                _enemyBase.Recycle();
            }

            _enemyBase = _baseFactory.GetBase(Faction.Enemy);
            _enemyBase.PrepareIntro(
                _enemyBasePoint, 
                _coinSpawner, 
                _baseDestructionManager);
        }

        private void RemoveBases()
        {
            _playerBase.Recycle();
            _enemyBase.Recycle();
        }
        
        private void UpdateData(BaseModel model) => _playerBase.UpdateData(model);

        private void CalculateBasePoints()
        {
            _enemyBasePoint = new Vector3(_cameraService.CameraWidth, 0, 0);
            _playerBasePoint = new Vector3(-_cameraService.CameraWidth, 0, 0);
        }

        public void Cleanup()
        {

        }
    }
}