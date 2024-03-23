using System;
using _Game.Core.Factory;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Bases.Factory
{
    [CreateAssetMenu(fileName = "Base Factory", menuName = "Factories/Base")]
    public class BaseFactory : GameObjectFactory, IBaseFactory
    {
        private IBattleStateService _battleState;
        private IAgeStateService _ageState;
        private IWorldCameraService _cameraService;

        public void Initialize(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IWorldCameraService cameraService)
        {
            _battleState = battleState;
            _ageState = ageState;
            _cameraService = cameraService;
        }
        
        public Base GetBase(Faction faction)
        {
            BaseData baseData;
            
            switch (faction)
            {
                case Faction.Player:
                    baseData = _ageState.GetForPlayerBase();
                    break;
                case Faction.Enemy:
                    baseData = _battleState.ForEnemyBase();
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
                _cameraService);
            
            return instance;
        }
        
        public void Reclaim(Base @base)
        {
            Destroy(@base.gameObject);
        }
    }
}