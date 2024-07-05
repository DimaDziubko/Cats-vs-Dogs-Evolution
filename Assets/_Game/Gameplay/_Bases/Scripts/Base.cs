using Assets._Game.Common;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets._Game.Gameplay._Bases.Scripts
{
    [RequireComponent(typeof(TargetPoint))]
    public class Base : GameBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Health _health;

        [SerializeField] private Collider2D _bodyCollider;
        [SerializeField] private TargetPoint _targetPoint;

        [SerializeField] private TowerDestructionAnimator _animator;

        [SerializeField] private DamageFlashEffect _damageFlash;

        private IBaseDestructionManager _baseDestructionManager;
        private ICoinSpawner _coinSpawner;
        private IInteractionCache _interactionCache;

        private Faction _faction;

        [ShowInInspector]
        private float _coinsPerBase;
        [ShowInInspector]
        private float _coinsPerHp;
        
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

        private Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = _health.HealthBarRotation = value;
        }

        public IBaseFactory OriginFactory { get; set; }
        
        public void Construct(
            Faction faction,
            float health,
            float coinsPerBase,
            IWorldCameraService cameraService,
            int baseLayer)
        {
            _bodyCollider.gameObject.layer = baseLayer;
            
            Rotation = faction == Faction.Enemy
                ? Quaternion.Euler(0, 180, 0) 
                : Quaternion.Euler(0, 0, 0);
            
            _coinsPerBase = coinsPerBase;

            _faction = faction;

            _health.Construct(
                health,
                cameraService);
            
            _targetPoint.Transform = _transform;
            _targetPoint.Damageable = _health;

            Subscribe();

            _animator.Construct();
            
            _damageFlash.Construct();
            
            HideHealth();
        }

        private void Subscribe()
        {
            _health.Death += OnBaseDeath;
            _health.Hit += OnBaseHit;
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

        public void UpdateData(BaseModel model) => _health.UpdateData(model.Health);

        public void ShowHealth() => _health.ShowHealth();

        private void HideHealth() => _health.HideHealth();

        public override void Recycle()
        {
            Unsubscribe();
            OriginFactory.Reclaim(this);
        }

        private void RegisterSelf()
        {
            InteractionCache.Register(_bodyCollider, _targetPoint);
        }

        private void OnBaseDeath()
        {
            _baseDestructionManager.BaseDestructionStarted(_faction, this);
            
            _animator.AnimationCompleted += HandleAnimationCompleted;
            _animator.StartDestructionAnimation();
            
            Unsubscribe();

            _damageFlash.Cleanup();
            
            HideHealth();
        }

        private void Unsubscribe()
        {
            _health.Death -= OnBaseDeath;
            _health.Hit -= OnBaseHit;
        }

        private void HandleAnimationCompleted()
        {
            
            _baseDestructionManager.BaseDestructionCompleted(_faction, this);
            
            _animator.AnimationCompleted -= HandleAnimationCompleted;
        }

        private void OnBaseHit(float damage, float maxHealth)
        {
            if(_faction == Faction.Player) return;

            if(_coinsPerHp <= 0) _coinsPerHp = _coinsPerBase / maxHealth;
             
            float coinsToDrop = damage * _coinsPerHp;

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