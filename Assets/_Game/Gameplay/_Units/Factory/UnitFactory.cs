using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Assets._Game.Core.DataPresenters.UnitDataPresenter;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using UnityEngine;

namespace Assets._Game.Gameplay._Units.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        private IWorldCameraService _cameraService;
        private IRandomService _random;
        private IUnitDataPresenter _unitDataPresenter;
        private ISoundService _soundService;


        private readonly Dictionary<(Faction, UnitType), Queue<Unit>> _unitsPools = 
            new Dictionary<(Faction, UnitType), Queue<Unit>>();

        public void Initialize(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService,
            IUnitDataPresenter unitDataPresenter)
        {
            _cameraService = cameraService;
            _random = random;
            _unitDataPresenter = unitDataPresenter;
            _soundService = soundService;
        }
        
        public Unit Get(Faction faction, UnitType type)
        {
            UnitData unitData = faction == Faction.Player 
                ? _unitDataPresenter.GetUnitData(type, Constants.CacheContext.AGE) 
                : _unitDataPresenter.GetUnitData(type, Constants.CacheContext.BATTLE);

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