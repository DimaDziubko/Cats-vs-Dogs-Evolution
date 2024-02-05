using System.Linq;
using _Game.Gameplay._Unit._Target;
using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    public class UnitAttackDetection : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 2.0f;
        
        [SerializeField] private TriggerObserver _triggerObserver;

        public bool HasTarget => (_currentAttackTarget!= null 
                                  && _currentAttackTarget.Transform != null 
                                  && _currentAttackTarget.Damageable.IsDead == false);

        public Vector3 TargetPosition
        {
            get
            {
                if (_currentAttackTarget.Transform != null)
                {
                    return _currentAttackTarget.Transform.position;
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
                if (_currentAttackTarget.Damageable != null)
                {
                    return _currentAttackTarget.Damageable;
                }
                else
                {
                    return null;
                }
            }
        }

        private Target _currentAttackTarget;
        
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
            if (targetTransform == _currentAttackTarget.Transform)
            {
                UpdateCurrentTarget();
            }
        }

        private void UpdateCurrentTarget()
        {
            _currentAttackTarget = _targetCollection.Targets
                .Where(t => !t.Damageable.IsDead)
                .OrderBy(t => (t.Transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
            
            _currentAttackTarget =  _currentAttackTarget?.Transform != null ?  _currentAttackTarget : null;
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
