using System;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ToggleWithSpriteSwap : MonoBehaviour, IFeature
    {
        [SerializeField] private Sprite _offSprite;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Image _changableImage;

        private Button _button;

        private bool _isOn;

        public Feature Feature => Feature.Pause;

        public event Action<bool> ValueChanged;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _isOn = false;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void SetActive(bool isAvailable) => 
            gameObject.SetActive(isAvailable);

        public void UpdateToggleStateManually(bool isOn)
        {
            _isOn = isOn;
            _changableImage.sprite = _isOn ? _onSprite : _offSprite;
        }

        private void OnButtonClicked()
        {
            _isOn = !_isOn;
            _changableImage.sprite = _isOn ? _onSprite : _offSprite;
            ValueChanged?.Invoke(_isOn);
        }
    }
}