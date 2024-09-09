using System;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItemView : MonoBehaviour
    {
        public event Action<UnitType, float> Upgrade;
        public event Action<float> TryUpgrade;
        public event Action<UnitType> InfoClicked;
        
        [SerializeField] private Image _backgroundHolder;
        
        [SerializeField] private Sprite _lockedItemImage;
        [SerializeField] private Sprite _unlockedItemImage;
        
        [SerializeField] private Image _unitIconHolder;

        [SerializeField] private TransactionButton _transactionButton;
        [SerializeField] private TMP_Text _unitNameLabel;

        [SerializeField] private Button _infoButton;

        [SerializeField] private StatsInfoPanel _statsInfoPanel;
        
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

        private void Subscribe()
        {
            _transactionButton.Click += OnTransactionButtonClick;
            _transactionButton.InactiveClick += OnInactiveButtonClick;
            _infoButton.onClick.AddListener(OnInfoButtonClicked);
        }

        private void OnInfoButtonClicked()
        {
            InfoClicked?.Invoke(_type);
            PlayButtonSound();
        }

        private void Unsubscribe()
        {
            _transactionButton.Click -= OnTransactionButtonClick;
            _transactionButton.InactiveClick -= OnInactiveButtonClick;
            _infoButton.onClick.RemoveAllListeners();
        }

        private void OnInactiveButtonClick() => 
            TryUpgrade?.Invoke(_price);

        private void OnTransactionButtonClick()
        {
            PlayButtonSound();
            Upgrade?.Invoke(_type, _price);
        }

        public void UpdateUI(UnitUpgradeItemModel model)
        {
            if (model.IsBought)
            {
                HandleUnlockedState(model);
                return;
            }
            
            HandleLockedState(model);
        }

        private void HandleLockedState(UnitUpgradeItemModel model)
        {
            _transactionButton.Show();
            _unitIconHolder.sprite = model.WarriorIcon;
            _type = model.Type;
            _price = model.Price;
            _backgroundHolder.sprite = _lockedItemImage;
            _unitNameLabel.enabled = false;
            _transactionButton.UpdateButtonState(model.ButtonState, model.Price.FormatMoney());
            _infoButton.interactable = false;
            _statsInfoPanel.UpdateView(model.Stats, false);
        }

        private void HandleUnlockedState(UnitUpgradeItemModel model)
        {
            _transactionButton.Hide();
            _unitIconHolder.sprite = model.WarriorIcon;
            _type = model.Type;
            _price = model.Price;
            _backgroundHolder.sprite = _unlockedItemImage;
            _unitNameLabel.enabled = true;
            _transactionButton.UpdateButtonState(model.ButtonState, model.Price.FormatMoney());
            _unitNameLabel.text = model.Name;
            _infoButton.interactable = true;
            _statsInfoPanel.UpdateView(model.Stats, true);
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