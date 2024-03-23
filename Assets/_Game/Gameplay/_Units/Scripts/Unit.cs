using System.Collections;
using System.Collections.Generic;
using _Game.Common;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.FSM;
using _Game.Gameplay._Units.FSM.States;
using _Game.Gameplay._Units.Scripts.Attack;
using _Game.Gameplay._Weapon.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class Unit : GameBehaviour
    {
        private const float RETURN_TO_POOL_DELAY = 3f;
        private readonly Vector3 _poolPosition = new Vector3(20f,0f,0f);  
        
        
        [SerializeField] private Transform _transform;

        [SerializeField] private UnitAnimator _animator;

        [SerializeField] private Health _health;

        [SerializeField] private UnitMove _move;

        [SerializeField] private TargetDetection _aggroDetection;

        [SerializeField] private TargetDetection _attackDetection;

        [SerializeField] private UnitAttack _attack;

        [SerializeField] private Collider2D _bodyCollider;

        [SerializeField] private TargetPoint _targetPoint;

        [SerializeField] private DamageFlashEffect _damageFlash;

        [SerializeField] private DynamicSortingOrder _dynamicSortingOrder;
        
        private WeaponType WeaponType { get; set; }

        public Faction Faction { get; private set; }

        public UnitType Type { get; private set; }

        private int _coinsPerKill;

        private IRandomService _random;

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        private Vector3 _destination;


        private Vector3 Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                _fsm.Enter<MoveToPointState, Vector3>(_destination);
            }
        }

        private float _tempAnimatorSpeedFactor;

        private float _tempMoveSpeedFactor;

        private UnitFsm _fsm;

        private bool _isDead;

        public IUnitFactory OriginFactory { get; set; }

        private IInteractionCache _interactionCache;
        
        private ICoinSpawner _coinSpawner;
        private IVFXProxy _vfxProxy;

        private IInteractionCache InteractionCache
        {
            get => _interactionCache;
            set
            {
                _interactionCache = value;
                _aggroDetection.InteractionCache = value;
                _attackDetection.InteractionCache = value;
                RegisterSelf();
            }
        }

        private void RegisterSelf()
        {
            InteractionCache.Register(_bodyCollider, _targetPoint);
        }

        public void Construct(
            WarriorConfig config,
            IWorldCameraService cameraService,
            Faction faction,
            UnitType type,
            IRandomService random,
            IAudioService audioService)
        {
            Faction = faction;
            Type = type;
            
            _random = random;

            WeaponType = config.WeaponConfig.WeaponType;
            _coinsPerKill = config.CoinsPerKill;
            
            InitializeFsm();

            _aggroDetection.Construct();
            _attackDetection.Construct();

            //TODO Config
            _attack.Construct(
                config.WeaponConfig,
                faction,
                audioService);

            _health.Construct(
                config.Health,
                cameraService);
            _move.Construct(
                transform,
                config.Speed);

            _damageFlash.Construct();
            
            _health.Death += OnDeath;
            _health.HideHealth();

            _targetPoint.Damageable = _health;
            _targetPoint.Transform = _transform;

            _animator.Construct();
        }


        public void Initialize(
            IInteractionCache interactionCache, 
            Vector3 playerSpawnPoint, 
            Vector3 destination,
            IShootProxy shootProxy,
            IVFXProxy vFXProxy,
            ICoinSpawner coinSpawner)
        {
            //TODO Delete
            Debug.Log($"{playerSpawnPoint} UNIT");
            
            InteractionCache = interactionCache;
            Position = playerSpawnPoint;
            Destination = destination;
            _attack.SetShootProxy(shootProxy);
            _attack.SetVFXProxy(vFXProxy);

            _coinSpawner = coinSpawner;
            _vfxProxy = vFXProxy;
        }

        public void ResetUnit()
        {
            _isDead = false;
            _health.ResetHealth();
            _health.HideHealth();
            _aggroDetection.Enable();
            _attackDetection.Enable();
            _fsm.Enter<IdleState>();
            _damageFlash.Reset();
        }

        public override bool GameUpdate()
        {
            if (_isDead)
            {
                Recycle();
                return false;
            }
            
            _fsm.GameUpdate();
            _dynamicSortingOrder.GameUpdate();
            return true;
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        private void InitializeFsm()
        {
            _fsm = new UnitFsm();

            IdleState idleState = new IdleState(_fsm);
            MoveToPointState moveState =
                new MoveToPointState(_fsm, _move, _aggroDetection, _attackDetection, _animator, _random);
            MoveToTargetState moveToTargetState =
                new MoveToTargetState(_fsm, _move, _aggroDetection, _attackDetection, _animator);
            AttackState attackState = new AttackState(_fsm, _animator, _attack, _aggroDetection, _attackDetection);
            DeathState deathState = new DeathState();

            _fsm.AddState(idleState);
            _fsm.AddState(moveState);
            _fsm.AddState(moveToTargetState);
            _fsm.AddState(attackState);
            _fsm.AddState(deathState);
            _fsm.Enter<IdleState>();
        }

        private void OnDeath()
        {
            //TODO Delete
            Debug.Log("OnUnitDeath");
            
            _vfxProxy.SpawnUnitVfx(Position);
            
            if (Faction == Faction.Enemy)
            {
                _coinSpawner.SpawnLootCoin(Position, _coinsPerKill);
            }
            
            _aggroDetection.Disable();
            _attackDetection.Disable();
            
            _fsm.Enter<IdleState>();
            
            Position = _poolPosition;
            StartCoroutine(ReturnToPoolAfterDelay());
        }
        
        IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(RETURN_TO_POOL_DELAY);
            
            //TODO Delete
            Debug.Log("Origin factory reclaim");
            _isDead = true;
        }
        
        private void OnDestroy()
        {
            _interactionCache.Unregister(_bodyCollider);
            _fsm.Cleanup();
            _aggroDetection.Cleanup();
            _health.Death -= OnDeath;
            _attackDetection.Cleanup();
            _attack.SetTarget(null);
            _damageFlash.Cleanup();
        }

        public override void SetPaused(in bool isPaused)
        {
            if (isPaused)
            {
                _tempAnimatorSpeedFactor = _animator.SpeedFactor;
                _tempMoveSpeedFactor = _move.SpeedFactor;
            }
            _animator.SetSpeedFactor(isPaused ? 0 : _tempAnimatorSpeedFactor);
            _move.SetSpeedFactor(isPaused ? 0 : _tempMoveSpeedFactor);
        }

        #region Helpers

#if UNITY_EDITOR
        [Button]
        private void ManualInit()
        {
            _animator = GetComponentInChildren<UnitAnimator>();
            _transform = GetComponent<Transform>();
            _dynamicSortingOrder = GetComponent<DynamicSortingOrder>();
            _health = GetComponent<Health>();
            _damageFlash = GetComponent<DamageFlashEffect>();
            _attack = GetComponentInChildren<UnitAttack>();
            _bodyCollider = GetComponent<Collider2D>();
            _move = GetComponent<UnitMove>();
            _targetPoint = GetComponent<TargetPoint>();
            _aggroDetection = GetComponents<TargetDetection>()[0];
            _attackDetection = GetComponents<TargetDetection>()[1];
        }
        [Button]
        private void Player()
        {
            SetLayerRecursive(transform, 8);  
            SetLayerForChildWithName(transform, "AggroZone", 10);  
            SetLayerForChildWithName(transform, "AttackZone", 12); 
        }
        [Button]
        private void Enemy()
        {
            SetLayerRecursive(transform, 9);  
            SetLayerForChildWithName(transform, "AggroZone", 11);  
            SetLayerForChildWithName(transform, "AttackZone", 13); 
        }

        private void SetLayerRecursive(Transform parent, int layer)
        {
            if (parent == null)
                return;

            parent.gameObject.layer = layer;

            foreach (Transform child in parent)
            {
                SetLayerRecursive(child, layer);
            }
        }

        private void SetLayerForChildWithName(Transform parent, string childName, int layer)
        {
            Transform child = parent.Find(childName);

            if (child != null)
            {
                child.gameObject.layer = layer;
            }
        }
    
#endif

        #endregion
    }

}