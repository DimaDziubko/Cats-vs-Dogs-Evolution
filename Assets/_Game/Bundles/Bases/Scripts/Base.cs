using _Game.Bundles.Bases.Factory;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Common;
using UnityEngine;

namespace _Game.Bundles.Bases.Scripts
{
    public class Base : GameBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Health _health;
        
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
        
        public void Construct(float health)
        {
            _health.Construct(health);
            
            //TODO Delete
            Debug.Log($"Enemy base health constructed {health}");
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }
    }
}