using System;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItemView : MonoBehaviour
    {
        public event Action<UnitType, float> Upgrade;
        
        [SerializeField] private Image _backgroundHolder;
        
        [SerializeField] private Sprite _lockedItemImage;
        [SerializeField] private Sprite _unlockedItemImage;
        
        [SerializeField] private Image _unitIconHolder;

        [SerializeField] private TransactionButton _transactionButton;
        [SerializeField] private TMP_Text _unitNameLabel;

        private IAudioService _audioService;
        
        [SerializeField] private UnitType _type;
        private float _price;

        public UnitType Type => _type;
        
        public void Init(IAudioService audioService)
        {
            _audioService = audioService;
            
            Unsubscribe();
            Subscribe();
            
            _transactionButton.Init();
        }

        private void Subscribe() => 
            _transactionButton.Click += OnTransactionButtonClick;

        private void Unsubscribe() => 
            _transactionButton.Click -= OnTransactionButtonClick;

        private void OnTransactionButtonClick()
        {
            PlayButtonSound();
            Upgrade?.Invoke(_type, _price);
        }

        public void UpdateUI(UnitUpgradeItemModel model)
        {
            
            if(!_transactionButton) return;

            _transactionButton.Show();
            
            _unitIconHolder.sprite = model.StaticData.Icon;
            _type = model.StaticData.Type;
            _price = model.StaticData.Price;
            
            if (model.IsBought)
            {
                _backgroundHolder.sprite = _unlockedItemImage;
                _transactionButton.Hide();
                _unitNameLabel.enabled = true;
                _unitNameLabel.text = model.StaticData.Name;
                return;
            }
            
            _backgroundHolder.sprite = _lockedItemImage;
            _unitNameLabel.enabled = false;
            _transactionButton.UpdateButtonState(model.CanAfford, model.StaticData.Price);
        }

        public void Cleanup()
        {
            Unsubscribe();
            _transactionButton.Cleanup();
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
    }
}