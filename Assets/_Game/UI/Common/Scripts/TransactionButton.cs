using System;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class TransactionButton : MonoBehaviour
    {
        public event Action Click;

        [SerializeField] private TMP_Text _priceText;
        
        private Button _button;

        private readonly Color _affordableColor = new Color(1f, 1f, 1f); 
        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        public void UpdateButtonState(bool canAfford, float price)
        {
            _button.interactable = canAfford;
            _priceText.text = price.FormatMoney();
            _priceText.color = canAfford ? _affordableColor : _expensiveColor;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _button.onClick.AddListener(() => Click?.Invoke());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Cleanup();
        }
        
        private void OnDisable()
        {
            //TODO Delete
            
            Debug.Log("TransitionButton cleanup");
            
            Cleanup();
        }

        private void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}