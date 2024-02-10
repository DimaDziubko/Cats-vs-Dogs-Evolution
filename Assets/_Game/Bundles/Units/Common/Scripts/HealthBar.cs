using UnityEngine;
using UnityEngine.UI;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void UpdateHealthView(float currentHealth, float maxHealth)
        {
            _slider.value = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }
}
