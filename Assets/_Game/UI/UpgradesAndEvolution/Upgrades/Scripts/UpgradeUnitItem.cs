using System;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItem : MonoBehaviour
    {
        private Action _clickAction;
        
        [SerializeField] private Image _backgroundHolder;
        
        [SerializeField] private Sprite _lockedItemImage;
        [SerializeField] private Sprite _unlockedItemImage;
        
        [SerializeField] private Image _unitIconHolder;

        [SerializeField] private TransactionButton _transactionButton;
        [SerializeField] private TMP_Text _unitNameLabel;

        public void Setup(int unitIndex, Action<int> onPurchaseRequested)
        {
            _clickAction = () => onPurchaseRequested.Invoke(unitIndex);
            _transactionButton.Click += _clickAction;
        }
        
        public void UpdateUI(bool isBought, bool canAfford, float price, Sprite unitIcon, string unitName)
        {
            _unitIconHolder.sprite = unitIcon;
            
            if (isBought)
            {
                _backgroundHolder.sprite = _unlockedItemImage;
                _transactionButton.Hide();
                _unitNameLabel.enabled = true;
                _unitNameLabel.text = unitName;
                return;
            }
            
            _backgroundHolder.sprite = _lockedItemImage;
            _transactionButton.Show();
            _unitNameLabel.enabled = false;
            _transactionButton.UpdateButtonState(canAfford, price);
        }

        private void OnDisable()
        {
            if (_transactionButton != null)
            {
                //TODO Delete
                Debug.Log("UpgradeUnitItem unsubscribed");
                _transactionButton.Click -= _clickAction;
            }
        }
    }
}