using System;
using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Core.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Utils;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using UnityEngine;

namespace _Game.Gameplay._Bases.Factory
{
    [CreateAssetMenu(fileName = "Base Factory", menuName = "Factories/Base")]
    public class BaseFactory : GameObjectFactory, IBaseFactory
    {
        private IBasePresenter _basePresenter;
        private IWorldCameraService _cameraService;

        public void Initialize(
            IBasePresenter basePresenter,
            IWorldCameraService cameraService)
        {
            _basePresenter = basePresenter;
            _cameraService = cameraService;
        }
        
        public Base GetBase(Faction faction)
        {
            BaseModel baseModel;
            
            switch (faction)
            {
                case Faction.Player:
                    baseModel = _basePresenter.GetBaseModel(Constants.CacheContext.AGE);
                    break;
                case Faction.Enemy:
                    baseModel = _basePresenter.GetBaseModel(Constants.CacheContext.BATTLE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, "Base data is null");
            }
            
            Base instance = CreateGameObjectInstance(baseModel.StaticData.BasePrefab);
            
            instance.OriginFactory = this;
            
            instance.Construct(
                faction, 
                baseModel.Health, 
                baseModel.StaticData.CoinsAmount, 
                _cameraService,
                baseModel.StaticData.Layer);
            
            return instance;
        }
        
        public void Reclaim(Base @base)
        {
            Destroy(@base.gameObject);
        }
    }
}