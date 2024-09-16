using _Game.Core.Services.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Image _colorIdentifier;
        [SerializeField] private GameObject _newNotification;
        [SerializeField] private CardViewAppearanceAnimation _appearanceAnimation;
        [SerializeField] private Image _coloredRippleImage;
        [SerializeField] private Image _maskImage;

        public void UpdateView(CardModel model)
        {
            _icon.sprite = model.Config.Icon;
            _levelLabel.text = model.Level;
            _newNotification.SetActive(model.IsNew);
            
            if (_colorIdentifier.material != model.Config.MaterialIdentifier)
            {
                _colorIdentifier.material = model.Config.MaterialIdentifier;
                _coloredRippleImage.material = model.Config.MaterialIdentifier;
                _maskImage.material = model.Config.MaterialIdentifier;
            }

            _coloredRippleImage.enabled = false;
            _maskImage.enabled = false;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void PlayAppearanceAnimation(
            IAudioService audioService, 
            Color flashColor, 
            bool needIlluminationAnimation,
            bool isNew)
        {
            _appearanceAnimation.Init(audioService, flashColor, isNew);
            _appearanceAnimation.Play(needIlluminationAnimation);
        }
    }
}