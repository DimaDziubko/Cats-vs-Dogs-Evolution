using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class CustomButtonPressAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private RectTransform _contentPanel;

        [SerializeField] private float _pressWarp = 0.9f;

        private Button _button;
        
        private float _normalHeight;
        private float _pressedHeight;

        private float _normalPanelPosition;
        private float _pressedPanelPosition;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _normalHeight = _buttonTransform.rect.height;
            _pressedHeight = _normalHeight * _pressWarp;
            
            if (_contentPanel != null)
            {
                _normalPanelPosition = _contentPanel.anchoredPosition.y;
                _pressedPanelPosition = 0;
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!_button.interactable) return;
            _buttonTransform.sizeDelta = new Vector2(_buttonTransform.sizeDelta.x, _pressedHeight);
            if(_contentPanel != null)
                _contentPanel.anchoredPosition = new Vector2(_contentPanel.anchoredPosition.x, _pressedPanelPosition);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _buttonTransform.sizeDelta = new Vector2(_buttonTransform.sizeDelta.x, _normalHeight);
            if(_contentPanel != null)
                _contentPanel.anchoredPosition = new Vector2(_contentPanel.anchoredPosition.x, _normalPanelPosition);
        }
        
#if UNITY_EDITOR
        //Helper
        [Button]
        private void ManualInit()
        {
            _buttonTransform = GetComponent<RectTransform>();
        }
#endif
    }
}