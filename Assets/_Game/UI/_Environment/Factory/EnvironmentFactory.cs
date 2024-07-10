using _Game.Gameplay._Battle.Scripts;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Camera;
using UnityEngine;

namespace Assets._Game.UI._Environment.Factory
{
    public interface IEnvironmentFactory
    {
        BattleEnvironment Get(BattleEnvironment battleEnvironment);
        void Reclaim(BattleEnvironment environment);
    }

    [CreateAssetMenu(fileName = "Environment Factory", menuName = "Factories/Environment")]
    public class EnvironmentFactory : GameObjectFactory, IEnvironmentFactory
    {
        [SerializeField] private EnvironmentSocket _socketPrefab;
        
        private IWorldCameraService _cameraService;

        private EnvironmentSocket _socketObject;
        
        public void Initialize(IWorldCameraService cameraService)
        {
            _cameraService = cameraService;
        }
        public BattleEnvironment Get(BattleEnvironment battleEnvironment)
        {
            if (_socketObject == null) _socketObject = GetSocket();
            
            var instance = CreateGameObjectInstance(battleEnvironment, _socketObject.EnvironmentAnchor);
            instance.OriginFactory = this;
            return instance;
        }

        private EnvironmentSocket GetSocket()
        {
            var instance = CreateGameObjectInstance(_socketPrefab);
            instance.OriginFactory = this;
            instance.Construct(_cameraService.MainCamera);
            return instance;
        }
        
        public void Reclaim(BattleEnvironment environment)
        {
            Destroy(environment.gameObject);
        }
    }
}