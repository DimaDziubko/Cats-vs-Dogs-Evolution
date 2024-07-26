using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Creatives.Creative_1.Scenario;
using Assets._Game.Gameplay._Units.Factory;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace Assets._Game.Creatives.Factories
{
    [CreateAssetMenu(fileName = "CrUnitFactory", menuName = "CrFactories/Unit")]
    public class CrUnitFactory : GameObjectFactory, IUnitFactory
    {
        private IWorldCameraService _cameraService;
        private IRandomService _random;
        private ISoundService _soundService;

        private readonly Dictionary<(Faction, UnitType), Queue<Unit>> _unitsPools = 
            new Dictionary<(Faction, UnitType), Queue<Unit>>();
        
        public void Initialize(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService)
        {
            _cameraService = cameraService;
            _random = random;
            _soundService = soundService;
            //TODO add SoundService
        }
        
        public Unit Get(Faction faction, UnitType type)
        {
            UnitData unitData = faction == Faction.Player ? CrSceneContext.I.ForPlayerUnit(type) : CrSceneContext.I.GetEnemy(type);

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
                    unitData.Config, 
                    _cameraService, 
                    faction, 
                    type,
                    _random,
                    _soundService,
                    unitData.UnitLayer,
                    unitData.AggroLayer,
                    unitData.AttackLayer);
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
        }
    }
}