using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class QuickBoostInfoPanel : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private BoostInfoItem[] _items;

        public BoostSource Source { get; set; }

        private IBoostDataPresenter _boostDataPresenter;
        private IAudioService _audioService;

        public void Construct(
            IBoostDataPresenter boostDataPresenter,
            IAudioService audioService,
            BoostSource source)
        {
            _boostDataPresenter = boostDataPresenter;
            _audioService = audioService;
            Source = source;
        }

        public void Init()
        {
            _boostDataPresenter.BoostModelChanged += OnBoostModelChanged;
            _button.onClick.AddListener(OnButtonClicked);
            foreach (var item in _items)
            {
                UpdateItemView(item);
            }
        }

        private void OnBoostModelChanged(BoostType type)
        {
            foreach (var item in _items)
            {
                if (item.BoostType == type)
                {
                    UpdateItemView(item);
                }
            }
        }

        private void UpdateItemView(BoostInfoItem item)
        {
            var value = _boostDataPresenter.TryGetBoostFor(BoostSource.TotalBoosts, item.BoostType);
            if(value != null) item.UpdateView(value, false);
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
            _boostDataPresenter.BoostModelChanged -= OnBoostModelChanged;
        }

        private void OnButtonClicked()
        {
            PlayButtonSound();
            _boostDataPresenter.ShowBoosts(Source);
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
    
}