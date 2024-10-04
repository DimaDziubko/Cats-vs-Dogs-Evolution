using System;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core.Factory;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Bases.Factory
{
    [CreateAssetMenu(fileName = "Base Factory", menuName = "Factories/Base")]
    public class BaseFactory : GameObjectFactory, IBaseFactory
    {
        private IBaseDataProvider _baseDataProvider;
        private IWorldCameraService _cameraService;

        public void Initialize(
            IBaseDataProvider baseDataProvider,
            IWorldCameraService cameraService)
        {
            _baseDataProvider = baseDataProvider;
            _cameraService = cameraService;
        }
        
        public Base GetBase(Faction faction)
        {
            IBaseData baseData;
            
            switch (faction)
            {
                case Faction.Player:
                    baseData = _baseDataProvider.GetBaseData(Constants.CacheContext.AGE);
                    break;
                case Faction.Enemy:
                    baseData = _baseDataProvider.GetBaseData(Constants.CacheContext.BATTLE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, "Base data is null");
            }
            
            Base instance = CreateGameObjectInstance(baseData.BasePrefab);
            
            instance.OriginFactory = this;
            
            instance.Construct(
                faction, 
                baseData.Health, 
                baseData.CoinsAmount, 
                _cameraService,
                baseData.Layer);
            
            return instance;
        }
        
        public void Reclaim(Base @base)
        {
            Destroy(@base.gameObject);
        }
    }
}