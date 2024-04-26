using System;
using System.Collections;
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
using _Game.Gameplay._Units.Scripts.Movement;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils.Extensions;
using Pathfinding.RVO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class Unit : GameBehaviour
    {
        private const float RETURN_TO_POOL_DELAY = 4f;

        private readonly Vector3 _playerPoolPosition = new Vector3(-20f,0f,0f);
        private readonly Vector3 _enemyPoolPosition = new Vector3(20f,0f,0f);


        [SerializeField] private Transform _transform;

        [SerializeField] private UnitAnimator _animator;

        [SerializeField] private Health _health;

        [SerializeField] private AUnitMove _aMove;

        [SerializeField] private TargetDetection _aggroDetection;

        [SerializeField] private TargetDetection _attackDetection;

        [SerializeField] private UnitAttack _attack;

        [SerializeField] private Collider2D _bodyCollider;

        [SerializeField] private TargetPoint _targetPoint;

        [SerializeField] private DamageFlashEffect _damageFlash;

        [SerializeField] private DynamicSortingOrder _dynamicSortingOrder;
        
        [SerializeField] private RVOController _rVOController;


        //Utils
        [SerializeField] private StateIndіcator _stateIndіcator;

        private WeaponType WeaponType { get; set; }

        public Faction Faction { get; private set; }

        public UnitType Type { get; private set; }

        private int _coinsPerKill;

        private IRandomService _random;


        private Vector3 Position
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
                _health.HealthBarRotation = Quaternion.Euler(0, _destination.x < Position.x ? 180 : 0, 0);
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
            IAudioService audioService,
            int unitLayer,
            int aggroLayer,
            int attackLayer)
        {
            SetupRVOLayers(faction);

            Faction = faction;
            Type = type;

            gameObject.layer = unitLayer;
            
            _random = random;

            WeaponType = config.WeaponConfig.WeaponType;
            _coinsPerKill = config.CoinsPerKill;


            _aggroDetection.Construct(aggroLayer);
            _attackDetection.Construct(attackLayer);
            
            _attack.Construct(
                config.WeaponConfig,
                faction,
                audioService,
                _transform);

            _health.Construct(
                config.GetUnitHealthForFaction(faction),
                cameraService);
            
            _aMove.Construct(
                transform,
                config.Speed);
            
            _damageFlash.Construct();

            _health.Death += OnDeath;
            _health.HideHealth();

            _targetPoint.Damageable = _health;
            _targetPoint.Transform = _transform;

            _animator.Construct(config.AttackPerSecond);
            
            InitializeFsm();
        }

        private void SetupRVOLayers(Faction faction)
        {
            if (faction == Faction.Enemy)
            {
                _rVOController.layer = RVOLayer.Layer3;
                _rVOController.collidesWith = RVOLayer.Layer3;
                return;
            }

            _rVOController.layer = RVOLayer.Layer2;
            _rVOController.collidesWith = RVOLayer.Layer2;
        }


        public void Initialize(
            IInteractionCache interactionCache, 
            Vector3 unitSpawnPoint, 
            Vector3 destination,
            IShootProxy shootProxy,
            IVFXProxy vFXProxy,
            ICoinSpawner coinSpawner)
        {
            InteractionCache = interactionCache;
            Position = unitSpawnPoint;
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
            _attack.Enable();
            _fsm.Enter<IdleState>();
            _damageFlash.Reset();
        }

        public override bool GameUpdate()
        {
            if(_stateIndіcator != null)
                _stateIndіcator.SetColor(_fsm.StateIndicator());
            
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
                new MoveToPointState(_fsm, _aMove, _aggroDetection, _attackDetection, _animator, _random);
            MoveToTargetState moveToTargetState =
                new MoveToTargetState(_fsm, _aMove, _aggroDetection, _attackDetection, _animator);
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
            _vfxProxy.SpawnUnitVfx(Position);
            
            if (Faction == Faction.Enemy)
            {
                _coinSpawner.SpawnLootCoin(Position, _coinsPerKill);
            }
            
            _aggroDetection.Disable();
            _attackDetection.Disable();
            _attack.Disable();

            _fsm.Enter<IdleState>();
            
            ReturnToPoolPosition();

            StartCoroutine(ReturnToPoolAfterDelay());
        }

        private void ReturnToPoolPosition()
        {
            switch (Faction)
            {
                case Faction.Player:
                    Position = _playerPoolPosition;
                    break;
                case Faction.Enemy:
                    Position = _enemyPoolPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(RETURN_TO_POOL_DELAY);
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
                _tempMoveSpeedFactor = _aMove.SpeedFactor;
            }
            _animator.SetSpeedFactor(isPaused ? 0 : _tempAnimatorSpeedFactor);
            _aMove.SetSpeedFactor(isPaused ? 0 : _tempMoveSpeedFactor);
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
            _rVOController = GetComponent<RVOController>();
            _damageFlash = GetComponent<DamageFlashEffect>();
            _attack = GetComponentInChildren<UnitAttack>();
            _bodyCollider = GetComponent<Collider2D>();
            _aMove = GetComponent<AUnitMove>();
            _targetPoint = GetComponent<TargetPoint>();
            _aggroDetection = GetComponents<TargetDetection>()[0];
            _attackDetection = GetComponents<TargetDetection>()[1];
        }
#endif
        #endregion
    }

}