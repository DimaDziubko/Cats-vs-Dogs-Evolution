using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using UnityEngine;

namespace _Game.UI._BoostPopup
{
    public class BoostUpgradeInfoPanel : MonoBehaviour
    {
        [SerializeField] private BoostUpgradeInfoItem[] _items;
        [SerializeField] private UIElementsFadeAnimation _animation;

        private IBoostDataPresenter _boostDataPresenter;
        private IAudioService _audioService;

        public void Construct(IBoostDataPresenter boostDataPresenter)
        {
            _boostDataPresenter = boostDataPresenter;
            gameObject.SetActive(false);
        }

        public void Init()
        {
            _boostDataPresenter.BoostModelChanged += OnBoostModelChanged;
            
            foreach (var item in _items)
            {
                item.Disable();
            }
        }

        private void OnBoostModelChanged(BoostType type)
        {
            foreach (var item in _items)
            {
                if (type == item.BoostType)
                {
                    item.Enable();
                    UpdateItemView(item);
                }
            }
            
            _animation.Play();
        }

        private void UpdateItemView(BoostUpgradeInfoItem item)
        {
            var value = _boostDataPresenter.TryGetBoostUpgradeInfoFor(item.BoostType);
            if (value != null)
            {
                item.UpdateView(value);
            }
        }

        public void Cleanup()
        {
            _boostDataPresenter.BoostModelChanged -= OnBoostModelChanged;
            _animation.Cleanup();
        }
        
    }
}