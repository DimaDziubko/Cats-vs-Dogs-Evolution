using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ToggleWithSpriteSwap : MonoBehaviour
    {
        [SerializeField] private Sprite _offSprite;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Image _changableImage;
        
        private Button _button;

        private bool _isOn;
        
        public event Action<bool> ValueChanged;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _isOn = false;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void UpdateToggleStateManually(bool isPaused)
        {
            _isOn = isPaused;
            _changableImage.sprite = _isOn ? _onSprite : _offSprite;
        }

        private void OnButtonClicked()
        {
            //TODO Delete
            Debug.Log("On toggle clicked");
            
            _isOn = !_isOn;
            _changableImage.sprite = _isOn ? _onSprite : _offSprite;
            ValueChanged?.Invoke(_isOn);
        }
    }
}