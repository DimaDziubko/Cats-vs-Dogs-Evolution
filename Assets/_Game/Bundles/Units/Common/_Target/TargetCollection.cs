using System.Collections.Generic;
using System.Linq;
using _Game.Bundles.Units.Common.Scripts;
using UnityEngine;

namespace _Game.Bundles.Units.Common._Target
{
    public class TargetCollection
    {
        private readonly List<Target> _targets = new List<Target>();
        
        public List<Target> Targets => _targets;

        public bool IsEmpty => _targets.Count == 0;
        
        public void Add(Transform transform, IDamageable damageable)
        {
            if (_targets.All(t => t.Transform != transform))
            {
                _targets.Add(new Target(transform, damageable));
            }
        }

        public void Remove(Transform targetTransform)
        {
            //TODO optimize
            _targets.RemoveAll(t => t.Transform == targetTransform);
        }

        public void UpdateTargets()
        {
            for (var i = 0; i < _targets.Count; i++)
            {
                if (_targets[i].Damageable.IsDead)
                {
                    var lastIndex = _targets.Count - 1;
                    if (i != lastIndex)
                    {
                        _targets[i] = _targets[lastIndex];
                    }
                    _targets.RemoveAt(lastIndex);
                }
            }
        }

        public void Clear()
        {
            _targets.Clear();
        }
    }
}