using System;
using _Game.Bundles.Units.Common.Scripts;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItem : MonoBehaviour
    {
        public event Action<UnitType> Upgrade;
        
        [SerializeField] private Image _backgroundHolder;
        
        [SerializeField] private Sprite _lockedItemImage;
        [SerializeField] private Sprite _unlockedItemImage;
        
        [SerializeField] private Image _unitIconHolder;

        [SerializeField] private TransactionButton _transactionButton;
        [SerializeField] private TMP_Text _unitNameLabel;

        [ShowInInspector]
        private UnitType _type;
        
        public void Init()
        {
            //TODO Delete
            Debug.Log("Inited transition button");
            _transactionButton.Init();
            _transactionButton.Click += OnTransactionButtonClick;
        }

        private void OnTransactionButtonClick()
        {
            Upgrade?.Invoke(_type);
        }

        public void UpdateUI(UnitType type, bool isBought, bool canAfford, float price, Sprite unitIcon, string unitName)
        {
            //TODO Delete
            Debug.Log($"Transition button is null{_transactionButton == null}") ;
            
            if(!_transactionButton) return;

            _transactionButton.Show();
            
            _unitIconHolder.sprite = unitIcon;
            _type = type;

            if (isBought)
            {
                _backgroundHolder.sprite = _unlockedItemImage;
                _transactionButton.Hide();
                _unitNameLabel.enabled = true;
                _unitNameLabel.text = unitName;
                return;
            }

            //TODO Delete
            Debug.Log("Update transition button");

            _backgroundHolder.sprite = _lockedItemImage;
            _unitNameLabel.enabled = false;
            _transactionButton.UpdateButtonState(canAfford, price);
        }

        public void Cleanup()
        {
            //TODO Delete
            Debug.Log("Clean transition button");
            _transactionButton.Click -= OnTransactionButtonClick;
            _transactionButton.Cleanup();
        }
    }
}