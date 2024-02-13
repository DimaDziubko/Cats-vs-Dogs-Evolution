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

        public void Init()
        {
            //TODO Delete
            Debug.Log("Transition button get component");
            _button = GetComponent<Button>();
            
            //TODO Delete
            Debug.Log($"Transition button INITED go is null {gameObject == null}, _button is null {_button == null}");
        }

        public void UpdateButtonState(bool canAfford, float price)
        {
                        
            //TODO Delete
            Debug.Log("Transition button Update");
            _button.interactable = canAfford;
            _priceText.text = price.FormatMoney();
            _priceText.color = canAfford ? _affordableColor : _expensiveColor;
            
            //TODO Delete
            Debug.Log($"Transition button UPDATED: go is null {gameObject == null}, _button is null {_button == null}");
        }

        public void Show()
        {
            //TODO Delete
            Debug.Log($"Transition button SHOW: go is null {gameObject == null}, _button is null {_button == null}");
            
            gameObject.SetActive(true);
            _button.onClick.AddListener(OnTransitionButtonClick);
        }

        public void Hide()
        {
            //TODO Fix bug!

            //TODO Delete
            Debug.Log("Transition button hide");
            
            //TODO Delete
            Debug.Log($"Transition button HIDE: go is null {gameObject == null}, _button is null {_button == null}");
            
            gameObject.SetActive(false);
            Cleanup();
        }

        public void Cleanup()
        {
            //TODO Delete
            Debug.Log("Transition button cleanup");
            
            //TODO Delete
            Debug.Log($"Transition button CLEANUP: go is null {gameObject == null}, _button is null {_button == null}");
            _button.onClick.RemoveAllListeners();
        }

        private void OnTransitionButtonClick()
        {
            Click?.Invoke();
        }
    }
}