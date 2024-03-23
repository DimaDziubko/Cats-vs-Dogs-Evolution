using System;
using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeItemView : MonoBehaviour
    {
        public event Action<UpgradeItemType> Upgrade;

        private IAudioService _audioService;
        
        [SerializeField] private Image _itemIconHolder;
        [SerializeField] private TMP_Text _itemNameLabel;
        [SerializeField] private TMP_Text _amountLabel;
        [SerializeField] private TransactionButton _transactionButton;
        
        [ShowInInspector]
        private UpgradeItemType _type;
        
        public void Init(IAudioService audioService)
        {
            _audioService = audioService;
            _transactionButton.Init();
            Unsubscribe();
            Subscribe();
        }

        private void Subscribe()
        {
            _transactionButton.Click += OnTransactionButtonClick;
        }
        
        private void Unsubscribe()
        {
            _transactionButton.Click -= OnTransactionButtonClick;
        }

        public void UpdateUI(UpgradeItemViewModel model)
        {
            if(!_transactionButton) return;

            _transactionButton.Show();
            
            _itemIconHolder.sprite = model.Icon;
            
            _type = model.Type;

            _amountLabel.text = model.AmountText;
            _itemNameLabel.text = model.Name;
            
            _transactionButton.UpdateButtonState(model.CanAfford, model.Price);
        }

        public void Cleanup()
        {
            //TODO Delete
            Debug.Log("Clean transition button");
            Unsubscribe();
            _transactionButton.Cleanup();
        }

        private void OnTransactionButtonClick()
        {
            PlayButtonSound();
            Upgrade?.Invoke(_type);
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}