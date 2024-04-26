using System;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeItemView : MonoBehaviour, ITutorialStep
    {
        public event Action<UpgradeItemType> Upgrade;

        private IAudioService _audioService;
        
        [SerializeField] private Image _itemIconHolder;
        [SerializeField] private TMP_Text _itemNameLabel;
        [SerializeField] private TMP_Text _amountLabel;
        [SerializeField] private TransactionButton _transactionButton;
        
        [ShowInInspector]
        private UpgradeItemType _type;

        [SerializeField] private TutorialStep _tutorialStep;
        public TutorialStep TutorialStep => _tutorialStep;
        public event Action<ITutorialStep> ShowTutorialStep;
        public event Action<ITutorialStep> CompleteTutorialStep;
        public event Action<ITutorialStep> BreakTutorial;

        public void Init(IAudioService audioService)
        {
            _audioService = audioService;
            _transactionButton.Init();
            Unsubscribe();
            Subscribe();
            ShowTutorialStep?.Invoke(this);
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
            Unsubscribe();
            _transactionButton.Cleanup();
            BreakTutorial?.Invoke(this);
        }

        private void OnTransactionButtonClick()
        {
            PlayButtonSound();
            Upgrade?.Invoke(_type);
            CompleteTutorialStep?.Invoke(this);
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}