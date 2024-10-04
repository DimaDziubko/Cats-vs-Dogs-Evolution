using System;
using _Game.Core.Services.Audio;
using _Game.UI.Factory;
using _Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardItemView : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private GameObject _upgradeNitifier;
        [SerializeField] private Animation _animation;
        [SerializeField] private CardView _cardView;

        [SerializeField] private Image _barFillImage;
        [SerializeField] private Color _normalBarColor;
        [SerializeField] private Color _upgradeBarColor;
        
        private ICardsPresenter _cardsPresenter;
        private IAudioService _audioService;
        public IUIFactory OriginFactory { get; set; }

        private int _id;

        public void Construct(
            ICardsPresenter cardsScreenPresenter,
            IAudioService audioService)
        {
            _cardsPresenter = cardsScreenPresenter;
            _audioService = audioService;
        }

        public void Init(int id)
        {
            _id = id;
            Unsubscribe();
            Subscribe();
        }

        public void UpdateView(CardModel model)
        {
            _progressLabel.text = model.Progress;
            _progressBar.value = model.ProgressValue;

            _cardView.UpdateView(model);
            
            bool needUpgrade = Math.Abs(model.ProgressValue - 1) <= Constants.ComparisonThreshold.MONEY_EPSILON;
            
            if (needUpgrade)
            {
                _upgradeNitifier.SetActive(true);
                _barFillImage.color = _upgradeBarColor;
                _animation.Play();
            }
            else
            {
                _barFillImage.color = _normalBarColor;
                _animation.Stop();
                _upgradeNitifier.SetActive(false);
            }
        }

        private void Subscribe() => 
            _upgradeBtn.onClick.AddListener(OnCardClicked);

        private void Unsubscribe() => 
            _upgradeBtn.onClick.RemoveAllListeners();

        private void OnCardClicked()
        {
            _cardsPresenter.OnCardClicked(_id);
            PlaySound();
        }

        public void Release()
        {
            OriginFactory.Reclaim(this);
        }

        private void PlaySound() => _audioService.PlayButtonSound();
    }
}
