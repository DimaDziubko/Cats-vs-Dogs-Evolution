using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Hud
{
    public class FoodBoostBtn : MonoBehaviour
    {
        [SerializeField] private Image _adsIconHolder;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private TMP_Text _foodAmountLabel;
        [SerializeField] private Image _foodIconHolder;

        [SerializeField] private Button _button;

        private bool _isSpent;
        
        public void Initialize(Action callback)
        {
            _isSpent = false;
            _button.onClick.AddListener(() => callback?.Invoke());
            _button.onClick.AddListener(OnButtonClicked);
            
            //TODO Delete
            Debug.Log($"FoodBoostBtn is INITIALIZED isSpent: {_isSpent}");
        }
        
        public void UpdateBtnState(FoodBoostBtnModel model)
        {
            //TODO Delete
            //TODO Fix bug
            Debug.Log($"FoodBoostBtn is UPDATED isSpent: {_isSpent}");
            
            gameObject.SetActive(!_isSpent && model.IsAvailable);

            _foodIconHolder.sprite = model.FoodIcon;
            _foodAmountLabel.text = model.FoodAmount.ToString();
            
            SetInteractable(model.IsInteractable);
        }

        private void OnButtonClicked()
        {
            _isSpent = true;
        }
        
        private void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _adsIconHolder.enabled = isInteractable;
            _foodAmountLabel.enabled = isInteractable;
            _foodIconHolder.enabled = isInteractable;
            
            _loadingText.enabled = !isInteractable;
        }
        
        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
        
    }
}