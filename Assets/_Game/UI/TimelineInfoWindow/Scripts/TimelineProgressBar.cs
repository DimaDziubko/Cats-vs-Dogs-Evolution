using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AgeMarker[] _ageMarkers;
        
        public void UpdateValue(int currentAge, int ages)
        {
            UpdateMarkers(currentAge);
            
            float percentage = 0;
            if (ages > 0)
            {
                 percentage = (float)currentAge / (ages - 1);
            }
            _slider.value = Mathf.Clamp01(percentage);
        }

        public Tween PlayValueAnimation(float newValue, float duration)
        {
           return _slider.DOValue(newValue, duration);
        }

        public void PlayMarkerRippleAnimation(in int nextAge, in float rippleAnimationDuration)
        {
            _ageMarkers[nextAge].PlayRippleAnimation(rippleAnimationDuration);
        }

        private void UpdateMarkers(in int currentAge)
        {
            for (int i = 0; i < _ageMarkers.Length; i++)
            {
                _ageMarkers[i].Initialize(i <= currentAge);
            }
        }
    }
}