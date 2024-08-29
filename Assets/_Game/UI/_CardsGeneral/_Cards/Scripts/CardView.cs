using _Game.UI.Factory;
using _Game.Utils;
using Assets._Game.Core.Services.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        
        [SerializeField] private Image _icon;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private Image _colorIdentifier;
        [SerializeField] private GameObject _upgradeNitifier;
        [SerializeField] private GameObject _newNotification;
        [SerializeField] private Animation _animation;
        
        private ICardsPresenter _cardsScreenPresenter;
        private IAudioService _audioService;
        public IUIFactory OriginFactory { get; set; }

        public void SetParent(Transform parent) => _transform.SetParent(parent);

        public void Construct(
            ICardsPresenter cardsScreenPresenter,
            IAudioService audioService)
        {
            _cardsScreenPresenter = cardsScreenPresenter;
            _audioService = audioService;
        }

        public void Init()
        {
            Unsubscribe();
            Subscribe();
        }

        public void UpdateView(CardModel model)
        {
            _icon.sprite = model.Config.Icon;
            _progressLabel.text = model.Progress;
            _levelLabel.text = model.Level;
            _progressBar.value = model.ProgressValue;
            _newNotification.SetActive(model.Config.IsNew);

            bool needUpgrade = model.ProgressValue - 1 <= Constants.ComparisonThreshold.MONEY_EPSILON;
            
            if (needUpgrade)
            {
                _upgradeNitifier.SetActive(true);
                _animation.Play();
            }
            else
            {
                Debug.Log("PLAY UPGRADE ANIMATION");
                _animation.Stop();
            }

            _colorIdentifier.color = model.Config.ColorIdentifier;
        }

        private void Subscribe()
        {
            _upgradeBtn.onClick.AddListener(OnCardClicked);
        }

        private void Unsubscribe()
        {
            _upgradeBtn.onClick.RemoveAllListeners();
        }

        private void OnCardClicked()
        {
            //TODO Implement later
        }

        public void Release()
        {
            OriginFactory.Reclaim(this);
        }
    }
}
