using System.Collections.Generic;
using System.Linq;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class InteractionCache : IInteractionCache
    {
        private readonly Dictionary<Collider2D, ITarget> _cache = new Dictionary<Collider2D, ITarget>();
        private float _timeSinceLastCleanup;
        
        public void Register(Collider2D collider, ITarget target)
        {
            _cache[collider] = target;
        }

        public void Unregister(Collider2D collider)
        {
            if (_cache.ContainsKey(collider))
            {
                _cache.Remove(collider);
            }
        }

        public ITarget Get(Collider2D key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            else
            {
                return null;
            }
        }
        
        public void Cleanup()
        {
            _cache.Clear();
        }
    }

    public interface IInteractionCache
    {
        ITarget Get(Collider2D key);
        void Register(Collider2D collider, ITarget target);
        void Unregister(Collider2D collider);
    }
}