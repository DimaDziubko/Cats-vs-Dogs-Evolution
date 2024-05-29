using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class ToggleButtonStateAnimator : MonoBehaviour
    {
        [SerializeField] private float _targetScaleX = 1;
        [SerializeField] private float _targetScaleY = 1;
        
        private RectTransform _buttonTransform;

        private Vector2 _normalButtonSize;
        private Vector2 _highlightedButtonSize;

        public void Initialize(RectTransform buttonTransform)
        {
            _buttonTransform = buttonTransform;
            _normalButtonSize = _buttonTransform.sizeDelta;
            _highlightedButtonSize = new Vector2(_normalButtonSize.x * _targetScaleX, _normalButtonSize.y * _targetScaleY);
        }

        public void Highlight()
        {
            _buttonTransform.sizeDelta = _highlightedButtonSize;
        }
        public void UnHighlight()
        {
            _buttonTransform.sizeDelta = _normalButtonSize;
        }
    }
}