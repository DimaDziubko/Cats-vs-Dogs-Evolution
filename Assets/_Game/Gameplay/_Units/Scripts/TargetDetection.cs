using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units._Target;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets._Game.Gameplay._Units.Scripts
{
    public class TargetDetection : MonoBehaviour
    {
        public event Action<ITarget> TargetUpdated;
        
        private const float UPDATE_INTERVAL = 2.0f;
        
        [SerializeField] private TriggerObserver _triggerObserver;

        public bool HasTarget => (_currentTarget!= null 
                                  && _currentTarget.Transform != null 
                                  && _currentTarget.Damageable.IsDead == false);

        public Vector3 TargetPosition
        {
            get
            {
                if (_currentTarget.Transform != null)
                {
                    return _currentTarget.Transform.position;
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
                if (_currentTarget.Damageable != null)
                {
                    return _currentTarget.Damageable;
                }
                else
                {
                    return null;
                }
            }
        }

        private ITarget _currentTarget;
        
        public IInteractionCache InteractionCache { get; set; }

        private readonly TargetCollection _targetCollection = new TargetCollection();
        private float _timeSinceLastUpdate;
        
        //Debug
        [ShowInInspector, ReadOnly]
        public List<ITarget> Targets => _targetCollection.Targets;

        public void Construct(
            int layer)
        {
            if (_triggerObserver != null)
            {
                _triggerObserver.Construct(layer);
                _triggerObserver.TriggerEnter += OnTargetEnter;
                _triggerObserver.TriggerExit += OnTargetExit;
            }
        }

        public void Construct(int layer, float radius)
        {
            Construct(layer);
            _triggerObserver.SetSize(radius);
        }
        public void Enable()
        {
            if (_triggerObserver)
            {
                _triggerObserver.enabled = true;
            }
        }

        public void Disable()
        {
            if (_triggerObserver)
            {
                _triggerObserver.enabled = false;
            }
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

        public void Cleanup()
        {
            if (_triggerObserver != null)
            {
                _triggerObserver.TriggerEnter -= OnTargetEnter;
                _triggerObserver.TriggerExit -= OnTargetExit;
            }
            
            _targetCollection.Clear();
        }

        private void OnTargetEnter(Collider2D targetCollider)
        {
            if(targetCollider == null) return;

            ITarget target = InteractionCache.Get(targetCollider);
            
            if (target != null)
            {
                _targetCollection.Add(target);
                UpdateCurrentTarget();
            }
        }

        private void OnTargetExit(Collider2D targetCollider)
        {
            if(targetCollider == null) return;

            var exitedTarget = InteractionCache.Get(targetCollider);
            if (exitedTarget != null) {
                _targetCollection.Remove(exitedTarget);
                UpdateCurrentTarget();
            }
        }

        private void UpdateCurrentTarget()
        {
            _currentTarget = _targetCollection.Targets
                .Where(t => !t.Damageable.IsDead && t.Transform != null)
                .OrderBy(t => (t.Transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
            
            _currentTarget = _currentTarget?.Transform != null ? _currentTarget : null;
            
            TargetUpdated?.Invoke(_currentTarget);
        }
    }
}
