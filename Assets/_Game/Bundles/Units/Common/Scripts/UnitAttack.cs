using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class UnitAttack : MonoBehaviour
    {
        private float _damage;

        private IDamageable _target;

        public IDamageable Target
        {
            set => _target = value;
        }
        
        public void Construct(float damage)
        {
            _damage = damage;
        }

        //Animation event
        private void OnAttack()
        {
            _target?.GetDamage(_damage);
        }
    }
}