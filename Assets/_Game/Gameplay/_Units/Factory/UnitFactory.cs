using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._Units.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        private IWorldCameraService _cameraService;
        private IRandomService _random;
        private IUnitDataProvider _unitDataProvider;
        private ISoundService _soundService;


        private readonly Dictionary<(Faction, UnitType), Queue<Unit>> _unitsPools = 
            new Dictionary<(Faction, UnitType), Queue<Unit>>();

        public void Initialize(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService,
            IUnitDataProvider unitDataProvider)
        {
            _cameraService = cameraService;
            _random = random;
            _unitDataProvider = unitDataProvider;
            _soundService = soundService;
        }
        
        public  Unit Get(Faction faction, UnitType type)
        {
            IUnitData unitData = faction == Faction.Player 
                ? _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE) 
                : _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.BATTLE);

            if (!_unitsPools.TryGetValue((faction, type), out Queue<Unit> pool))
            {
                pool = new Queue<Unit>();
                _unitsPools.Add((faction, type), pool);
            }

            Unit instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
                instance.Reset();
            }
            else
            {
                instance = CreateGameObjectInstance(unitData.Prefab);
                instance.OriginFactory = this;
                instance.Construct(
                    unitData,
                    _cameraService, 
                    faction,
                    _random,
                    _soundService);
            }
            
            return instance;
        }

        public async UniTask<Unit> GetAsync(Faction faction, UnitType type)
        {
            IUnitData unitData = faction == Faction.Player 
                ? _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE) 
                : _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.BATTLE);

            if (!_unitsPools.TryGetValue((faction, type), out Queue<Unit> pool))
            {
                pool = new Queue<Unit>();
                _unitsPools.Add((faction, type), pool);
            }

            Unit instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
                instance.Reset();
            }
            else
            {
                var key = unitData.Race == Race.Cat ? unitData.CatKey : unitData.DogKey;
                instance = await CreateGameObjectInstanceAsync<Unit>(key);
                instance.OriginFactory = this;
                instance.Construct(
                    unitData, 
                    _cameraService, 
                    faction,
                    _random,
                    _soundService);
            }
            
            return instance;
        }

        
        public void Reclaim(Unit unit)
        {
            unit.gameObject.SetActive(false); 
            
            var key = (unit.Faction, unit.Type);
            if (!_unitsPools.TryGetValue(key, out Queue<Unit> pool))
            {
                pool = new Queue<Unit>();
                _unitsPools.Add(key, pool);
            }
            
            pool.Enqueue(unit);
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _unitsPools.Values)
            {
                while (pool.Count > 0)
                {
                    var unit = pool.Dequeue();
                    Destroy(unit.gameObject);
                }
            }
            _unitsPools.Clear();
            
            base.Cleanup();
        }
    }
}