using System.Linq;
using _Game.Bundles.Units.Common._Target;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class UnitAggroDetection : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 2.0f;
        
        [SerializeField] private TriggerObserver _triggerObserver;
        
        public bool HasTarget => (_currentAggroTarget != null 
                                  && _currentAggroTarget.Transform != null 
                                  && _currentAggroTarget.Damageable.IsDead == false);

        public Vector3 TargetPosition
        {
            get
            {
                if (_currentAggroTarget.Transform != null)
                {
                    return _currentAggroTarget.Transform.position;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        
        public IDamageable TargetHealth
        {
            get
            {
                if (_currentAggroTarget.Damageable != null)
                {
                    return _currentAggroTarget.Damageable;
                }
                else
                {
                    return null;
                }
            }
        }

        private Target _currentAggroTarget;

        private readonly TargetCollection _targetCollection = new TargetCollection();
        private float _timeSinceLastUpdate;

        public void Construct()
        {
            _triggerObserver.TriggerEnter += OnTargetEnter;
            _triggerObserver.TriggerExit += OnTargetExit;
        }

        public void GameUpdate()
        {
            if(_targetCollection.IsEmpty) return;
            
            _timeSinceLastUpdate += Time.deltaTime;
            
            if (_timeSinceLastUpdate >= UPDATE_INTERVAL)
            {
                _targetCollection.UpdateTargets();
                UpdateCurrentTarget();
                _timeSinceLastUpdate = 0.0f;
            }
        }

        private void OnTargetEnter(Collider targetCollider)
        {
            if (targetCollider.TryGetComponent<IDamageable>(out var damageable))
            {
                _targetCollection.Add(targetCollider.transform, damageable);
                UpdateCurrentTarget();
            }
        }

        private void OnTargetExit(Collider targetCollider)
        {
            Transform targetTransform = targetCollider.transform;
            _targetCollection.Remove(targetTransform);
            if (targetTransform == _currentAggroTarget.Transform)
            {
                UpdateCurrentTarget();
            }
        }

        private void UpdateCurrentTarget()
        {
            _currentAggroTarget = _targetCollection.Targets
                .Where(t => !t.Damageable.IsDead)
                .OrderBy(t => (t.Transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
            
            _currentAggroTarget = _currentAggroTarget?.Transform != null ? _currentAggroTarget : null;
        }
        
        private void OnDestroy()
        {
            //TODO Delete 
            Debug.Log("Unsubscribed");
            
            _triggerObserver.TriggerEnter -= OnTargetEnter;
            _triggerObserver.TriggerExit -= OnTargetExit;
            _targetCollection.Clear();
        }
    }
}