using System;
using _Game.Common;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Units.Scripts.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Bases.Scripts
{
    [RequireComponent(typeof(TargetPoint))]
    public class Base : GameBehaviour
    {
        public event Action<Faction, Base> DestructionStarted;
        public event Action<Faction, Base> DestructionCompleted;
        
        [SerializeField] private Transform _transform;
        [SerializeField] private Health _health;

        [SerializeField] private Collider2D _bodyCollider;
        [SerializeField] private TargetPoint _targetPoint;

        [SerializeField] private BaseDestructionAnimator _animator;

        [SerializeField] private DamageFlashEffect _damageFlash;

        private IBaseDestructionManager _baseDestructionManager;
        private ICoinSpawner _coinSpawner;
        private IInteractionCache _interactionCache;

        private Faction _faction;

        private float _coinsPerBase;

        public IInteractionCache InteractionCache
        {
            get => _interactionCache;
            set
            {
                _interactionCache = value;
                RegisterSelf();
            }
        }

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

        public IBaseFactory OriginFactory { get; set; }
        
        public void Construct(
            Faction faction,
            float health,
            float coinsPerBase,
            IWorldCameraService cameraService)
        {
            _coinsPerBase = coinsPerBase;
            
            _faction = faction;
            
            _health.Construct(
                health,
                cameraService);

            _targetPoint.Transform = _transform;
            _targetPoint.Damageable = _health;

            _health.Death += OnBaseDeath;
            _health.Hit += OnBaseHit;

            _animator.Construct();
            
            _damageFlash.Construct();
            
            HideHealth();
        }

        public void PrepareIntro(
            Vector3 position, 
            ICoinSpawner coinSpawner, 
            IBaseDestructionManager baseDestructionManager)
        {
            _baseDestructionManager = baseDestructionManager;
            _coinSpawner = coinSpawner;
            Position = position;
        }

        public void UpdateData(BaseData data)
        {
            _health.UpdateData(data.Health);
        }

        public void ShowHealth()
        {
            _health.ShowHealth();
        }

        private void HideHealth()
        {
            _health.HideHealth();
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        private void RegisterSelf()
        {
            InteractionCache.Register(_bodyCollider, _targetPoint);
        }

        private void OnBaseDeath()
        {
            //DestructionStarted?.Invoke(_faction, this);

            _baseDestructionManager.BaseDestructionStarted(_faction, this);
            
            _animator.AnimationCompleted += HandleAnimationCompleted;
            _animator.StartDestructionAnimation();
            _health.Death -= OnBaseDeath;
            _health.Hit -= OnBaseHit;
            
            _damageFlash.Cleanup();
            
            HideHealth();
        }

        private void HandleAnimationCompleted()
        {
            //DestructionCompleted?.Invoke(_faction, this);

            _baseDestructionManager.BaseDestructionCompleted(_faction, this);
            
            _animator.AnimationCompleted -= HandleAnimationCompleted;
        }

        private void OnBaseHit(float percentage)
        {
            if(_faction == Faction.Player) return;

            float coinsToDrop = percentage * _coinsPerBase;

            if (coinsToDrop > _coinsPerBase)
            {
                coinsToDrop = _coinsPerBase;
            }
            
            _coinSpawner.SpawnLootCoin(Position, coinsToDrop);

            _coinsPerBase -= coinsToDrop;
        }

#if UNITY_EDITOR

        //Helper

        [Button]
        private void ManualInit()
        {
            _transform = GetComponent<Transform>();
            _damageFlash = GetComponent<DamageFlashEffect>();
            _health = GetComponentInChildren<Health>();
            _bodyCollider = GetComponentInChildren<Collider2D>();
            _targetPoint = GetComponent<TargetPoint>();
        }
#endif
    }
}