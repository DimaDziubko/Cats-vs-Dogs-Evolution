using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        private const float EPSILON = 0.01f;
    
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _effectBar;
        [SerializeField] private float _effectDelay = 0.5f;
        [SerializeField] private float _effectDuration = 1.0f;
    
        private Coroutine _effectBarCoroutine;
    
        public void UpdateHealthView(float currentHealth, float maxHealth)
        {
            if (_healthBar == null || _effectBar == null)
            {
                return;
            }

            float targetValue = Mathf.Clamp01(currentHealth / maxHealth);

            if (_effectBarCoroutine != null)
            {
                StopCoroutine(_effectBarCoroutine);
                _effectBar.value = _healthBar.value;
            }

            _healthBar.value = targetValue;
        
            _effectBarCoroutine = StartCoroutine(UpdateEffectBarWithDelay(targetValue, _effectDelay));
        }
    
        private IEnumerator UpdateEffectBarWithDelay(float targetValue, float delay)
        {
            yield return new WaitForSeconds(delay);
        
            while (_effectBar != null && Mathf.Abs(_effectBar.value - targetValue) > EPSILON)
            {
                _effectBar.value = Mathf.MoveTowards(_effectBar.value, targetValue, Time.deltaTime / _effectDuration);
                yield return null; 
            }

            if (_effectBar != null)
            {
                _effectBar.value = targetValue;
            }
        }
    }
}
