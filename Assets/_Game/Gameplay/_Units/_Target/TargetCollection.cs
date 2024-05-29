using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units._Target
{
    public class TargetCollection
    {
        public List<ITarget> Targets => _targets;
        public bool IsEmpty => _targets.Count == 0;
        
        private readonly List<ITarget> _targets = new List<ITarget>();

        public void Add(ITarget target) => 
            _targets.Add(target);

        public void Remove(ITarget target) => 
            _targets.Remove(target);

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

        public void Clear() => _targets.Clear();
    }
}