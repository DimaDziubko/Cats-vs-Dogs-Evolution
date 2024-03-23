using System;
using _Game.Core.Services.Camera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private Collider2D _bodyCollider;
        
        public event Action Death;
        public event Action<float> Hit;
        
        private float _maxHealth;
        private float _currentHealth;

        public void Construct(
            float health,
            IWorldCameraService cameraService)
        {
            _healthBar.Construct(cameraService);
            
            UpdateData(health);
        }

        public void ResetHealth()
        {
            UpdateData(_maxHealth);
        }

        public void UpdateData(float health)
        {
            _maxHealth = _currentHealth = health;
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
            _bodyCollider.enabled = true;
        }

        public bool IsDead => _currentHealth <= 0;

        public void HideHealth()
        {
            _healthBar.Hide();
        }

        public void ShowHealth()
        {
            _healthBar.Show();
        }
        
        public void GetDamage(float damage)
        {
            if(IsDead) return;
            
            ShowHealth();

            _currentHealth -= damage;

            if (IsDead)
            {
                _currentHealth = 0;
                Death?.Invoke();
                
                if (_bodyCollider != null)
                {
                    _bodyCollider.enabled = false;
                }
            }
            
            float percentage = damage / _maxHealth;
            
            Hit?.Invoke(percentage);
            
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
        }

        #if UNITY_EDITOR
        //Helper
        [Button]
        private void ManualInit()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _bodyCollider = GetComponent<Collider2D>();
        }
        #endif
    }
}