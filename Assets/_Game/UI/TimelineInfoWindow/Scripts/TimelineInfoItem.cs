using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineInfoItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _iconTransform;
        [SerializeField] private Image _ageIconHolder;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _dateRangeLabel;
        [SerializeField] private TMP_Text _descriptionLabel;

        [SerializeField] private TMP_Text _lockText;
        
        //Animation data
        [SerializeField] private float _animationScale = 1.2f;
        [SerializeField] private float _animationFade = 0.5f;
        [SerializeField] private int _loopCount = 2;

        public void UpdateModel(TimelineInfoItemModel model)
        {
            _nameLabel.text = model.Name; 
            _ageIconHolder.sprite = model.AgeIcon;
            _dateRangeLabel.text = model.DateRange;
            _descriptionLabel.text = model.Description;

            HandleLock(model.IsUnlocked);
        }

        public void PlayRippleAnimation(in float rippleAnimationDuration)
        {
            _iconTransform.DOScale(_animationScale, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
            _ageIconHolder.DOFade(_animationFade, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
        }

        private void HandleLock(bool isUnlocked)
        {
            _ageIconHolder.enabled = isUnlocked;
            _nameLabel.enabled = isUnlocked;
            _dateRangeLabel.enabled = isUnlocked;
            _descriptionLabel.enabled = isUnlocked;

            _lockText.enabled = !isUnlocked;
        }
    }
}