using System;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class UnitHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthBar _healthBar;
        
        public event Action Death;
        
        private float _maxHealth;
        private float _currentHealth;

        public void Construct(float health)
        {
            _maxHealth = _currentHealth = health;
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
        }

        public bool IsDead
        {
            get => _currentHealth <= 0;
        }

        public void GetDamage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Death?.Invoke();
            }

            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
        }
    }
}