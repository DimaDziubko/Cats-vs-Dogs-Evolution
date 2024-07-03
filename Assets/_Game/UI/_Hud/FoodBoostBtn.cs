using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI._Hud
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
            _button.onClick.AddListener(() =>
            {
                callback?.Invoke();
                OnButtonClicked();
            });
        }
        
        public void UpdateBtnState(FoodBoostBtnModel model)
        {
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

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}