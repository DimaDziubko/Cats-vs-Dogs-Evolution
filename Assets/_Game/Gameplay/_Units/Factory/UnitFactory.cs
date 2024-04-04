using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        private IBattleStateService _battleState;
        private IAgeStateService _ageState;
        private IWorldCameraService _cameraService;
        private IRandomService _random;
        private IAudioService _audioService;

        private readonly Dictionary<(Faction, UnitType), Queue<Unit>> _unitsPools = 
            new Dictionary<(Faction, UnitType), Queue<Unit>>();
        
        public void Initialize(
            IBattleStateService battleState,
            IAgeStateService ageState,
            IWorldCameraService cameraService,
            IRandomService random,
            IAudioService audioService)
        {
            _battleState = battleState;
            _ageState = ageState;
            _cameraService = cameraService;
            _random = random;
            _audioService = audioService;
        }
        
        public Unit Get(Faction faction, UnitType type)
        {
            if (_unitsPools != null && _unitsPools.ContainsKey((Faction.Player, UnitType.Light)))
            {
                //TODO Delete 
                Debug.Log($"Unit pool light count {_unitsPools[(Faction.Player, UnitType.Light)].Count}");
            }

            UnitData unitData = faction == Faction.Player ? _ageState.GetPlayerUnit(type) : _battleState.GetEnemy(type);

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
                instance.ResetUnit();
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
                    _audioService,
                    unitData.UnitLayer,
                    unitData.AggroLayer,
                    unitData.AttackLayer);
            }
            
            return instance;
        }

        public void Reclaim(Unit unit)
        {
            //TODO Delete
            Debug.Log("Factory reclaim");
            
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