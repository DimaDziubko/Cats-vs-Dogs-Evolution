using System;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private Collider2D _bodyCollider;
        
        public event Action Death;
        
        private float _maxHealth;
        private float _currentHealth;

        public void Construct(float health)
        {
            _maxHealth = _currentHealth = health;
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
            _bodyCollider.enabled = true;
        }

        public bool IsDead => _currentHealth <= 0;

        public void GetDamage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Death?.Invoke();
                
                if (_bodyCollider != null)
                {
                    _bodyCollider.enabled = false;
                }
            }

            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
        }
    }
}