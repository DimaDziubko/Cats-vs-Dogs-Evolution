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
        public event Action<ButtonState> ButtonStateChanged;
        public event Action<UpgradeItemType, float> Upgrade;

        private IAudioService _audioService;
        
        [SerializeField] private Image _itemIconHolder;
        [SerializeField] private TMP_Text _itemNameLabel;
        [SerializeField] private TMP_Text _amountLabel;
        [SerializeField] private TransactionButton _transactionButton;
        
        [ShowInInspector]
        private UpgradeItemType _type;

        private float _price;
        
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
            _transactionButton.ButtonStateChanged += OnButtonStateChanged;
        }

        private void OnButtonStateChanged(ButtonState state)
        {
            ButtonStateChanged?.Invoke(state);
        }

        private void Unsubscribe()
        {
            _transactionButton.Click -= OnTransactionButtonClick;
            _transactionButton.ButtonStateChanged += OnButtonStateChanged;
        }

        public void UpdateUI(UpgradeItemModel model)
        {
            if(!_transactionButton) return;

            _transactionButton.Show();
            
            _itemIconHolder.sprite = model.StaticData.Icon;
            
            _type = model.StaticData.Type;
            _price = model.DynamicData.Price;
            
            _amountLabel.text = model.AmountText;
            _itemNameLabel.text = model.StaticData.Name;
            
            _transactionButton.UpdateButtonState(model.CanAfford, model.DynamicData.Price);
        }

        public void Cleanup()
        {
            Unsubscribe();
            _transactionButton.Cleanup();
        }

        private void OnTransactionButtonClick()
        {
            PlayButtonSound();
            Upgrade?.Invoke(_type, _price);
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
    }
}