using System;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._BoostPopup;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _cardNameLabel;
        [SerializeField] private TMP_Text _cardTypeLabel;
        [SerializeField] private TMP_Text _cardDescriptionLabel;
        [SerializeField] private CardItemView cardItemView;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button[] _cancelButtons;
        [SerializeField] private UpgradeInfoItem[] _infoItems;
        [SerializeField] private BoostUpgradeInfoPanel _upgradeInfoPanel;
        
        private ICardsPresenter _cardPresenter;
        private IAudioService _audioService;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private int _currentCardId;

        public void Construct(
            IWorldCameraService cameraService,
            ICardsPresenter cardsPresenter,
            IBoostDataPresenter boostDataPresenter,
            IAudioService audioService)
        {
            _canvas.enabled = false;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _cardPresenter = cardsPresenter;
            _audioService = audioService;
            _upgradeInfoPanel.Construct(boostDataPresenter);
            Init();
        }

        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
            
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            _upgradeInfoPanel.Init();
            Unsubscribe();
            Subscribe();
        }

        private void Subscribe()
        {
            _cardPresenter.CardModelUpdated += OnCardModelUpdated;
        }

        private void OnCardModelUpdated(int id, CardModel _)
        {
            if(id != _currentCardId) return;
            UpdateViews(id);
        }

        private void Unsubscribe()
        {
            _cardPresenter.CardModelUpdated -= OnCardModelUpdated;
        }
        
        public async UniTask<bool> ShowDetailsAndAwaitForExit(int id)
        {
            _currentCardId = id;
            UpdateViews(id);
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void UpdateViews(int id)
        {
            var model = _cardPresenter.CardModels[id];
            cardItemView.UpdateView(model);

            _cardNameLabel.text = model.Config.Name;
            _cardTypeLabel.text = model.Config.Type.ToString();
            _cardTypeLabel.color = model.Config.ColorIdentifier;
            _cardDescriptionLabel.text = model.Config.Derscription;
            
            UpdateButtonAndBoostsInfo(model);
        }

        private void UpdateButtonAndBoostsInfo(CardModel model)
        {
            _upgradeButton.interactable = Math.Abs(model.ProgressValue - 1) <= Constants.ComparisonThreshold.MONEY_EPSILON;
            
            var boostsCount = model.BoostItemModels.Count;
            if (boostsCount == 0) return;

            for (int i = 0; i < _infoItems.Length; i++)
            {
                if (i < boostsCount)
                {
                    _infoItems[i].UpdateView(model.BoostItemModels[i]);
                    _infoItems[i].Enable();
                    continue;
                }

                _infoItems[i].Disable();
            }
        }

        private void OnUpgradeButtonClicked()
        {
            _cardPresenter.UpgradeCard(_currentCardId);
            PlayButtonSound();
            PlayUpgradeSound();
        }

        private void PlayUpgradeSound()
        {
            _audioService.PlayUpgradeSound();
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
        
        private void OnCancelled()
        {
            PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        public void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            _upgradeButton.onClick.RemoveAllListeners();
            
            Unsubscribe();
            
            _upgradeInfoPanel.Cleanup();
        }
    }
}