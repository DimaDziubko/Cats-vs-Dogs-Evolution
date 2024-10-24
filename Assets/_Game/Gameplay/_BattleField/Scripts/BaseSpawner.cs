﻿using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
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
        private readonly IBaseDataProvider _baseDataProvider;

        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;

        private Base _enemyBase, _playerBase;

        public BaseSpawner(
            IBaseFactory baseFactory,
            IWorldCameraService cameraService,
            ICoinSpawner coinSpawner,
            IBaseDestructionManager baseDestructionManager,
            IInteractionCache interactionCache,
            IBaseDataProvider baseDataProvider)
        {
            _baseFactory = baseFactory;
            _cameraService = cameraService;
            _coinSpawner = coinSpawner;
            _baseDestructionManager = baseDestructionManager;
            _interactionCache = interactionCache;
            _baseDataProvider = baseDataProvider;
        }

        public void OnStartBattle()
        {
            _enemyBase.InteractionCache = _interactionCache;
            _playerBase.InteractionCache = _interactionCache;
            _playerBase.UpdateHealth(_baseDataProvider.GetBaseData(Constants.CacheContext.AGE).Health);
            _enemyBase.UpdateHealth(_baseDataProvider.GetBaseData(Constants.CacheContext.BATTLE).Health);
            _enemyBase.ShowHealth();
            _playerBase.ShowHealth();
            
        }

        public void Init()
        {
            CalculateBasePoints();
            UpdatePlayerBase();
            
            _baseDataProvider.BaseUpdated += UpdateBase;
        }

        private void UpdateBase(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    RemoveBase(faction);
                    UpdatePlayerBase();
                    break;
                case Faction.Enemy:
                    RemoveBase(faction);
                    UpdateEnemyBase();
                    break;
            }
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

        public void SetPaused(bool isPaused)
        {
            _enemyBase.SetPaused(isPaused);
            _playerBase.SetPaused(isPaused);
        }

        private void RemoveBase(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    _playerBase.Recycle();
                    _playerBase = null;
                    break;
                case Faction.Enemy:
                    _enemyBase.Recycle();
                    _enemyBase = null;
                    break;
            }
        }

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