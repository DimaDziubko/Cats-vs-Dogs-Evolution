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

        //Animation data
        [SerializeField] private float _animationScale = 1.2f;
        [SerializeField] private float _animationFade = 0.5f;
        [SerializeField] private int _loopCount = 2;

        public void Initialize(TimelineInfoItemModel model)
        {
            _nameLabel.text = model.Name; 
            _ageIconHolder.sprite = model.AgeIcon;
            _dateRangeLabel.text = model.DateRange;
            _descriptionLabel.text = model.Description;
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
    }
}