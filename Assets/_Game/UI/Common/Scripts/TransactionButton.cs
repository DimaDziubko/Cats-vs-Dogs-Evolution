using System;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button), typeof(CustomButtonPressAnimator))]
    public class TransactionButton : MonoBehaviour
    {
        public event Action Click;

        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _infoText;
        
        private Button _button;
        
        private readonly Color _affordableColor = new Color(1f, 1f, 1f);

        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        public void Init()
        {
            //TODO Delete
            Debug.Log("Transition button get component");
            
            _button = GetComponent<Button>();
            
            Unsubscribe();
            Subscribe();
        }

        public void UpdateButtonState(bool canAfford, float price)
        {
            _button.interactable = canAfford;

            if (_priceText != null)
            {
                _priceText.text = price.FormatMoney();
                _priceText.color = canAfford ? _affordableColor : _expensiveColor;
            }
            if (_infoText != null)
            {
                _infoText.color = canAfford ? _affordableColor : _expensiveColor;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Cleanup();
        }

        public void Cleanup()
        {
            //TODO Delete
            Debug.Log("Transition button cleanup");
            Unsubscribe();
        }

        private void Subscribe()
        {
            _button.onClick.AddListener(OnTransitionButtonClick);
        }

        private void Unsubscribe()
        {
            if (!_button)
            {
                _button = GetComponent<Button>();
            }

            _button.onClick.RemoveAllListeners();
        }

        private void OnTransitionButtonClick()
        {
            Click?.Invoke();
        }
    }
}