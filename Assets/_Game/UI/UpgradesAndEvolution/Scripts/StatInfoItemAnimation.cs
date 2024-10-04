using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class StatInfoItemAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemTransform;
        [SerializeField] private Sprite _boostArrow;
        [SerializeField] private TMP_Text _statFullValueLabel;
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private float _spriteChangeDuration = 1f;

        [SerializeField] private Vector2 _normalScale = new Vector2(1f, 1f);
        [SerializeField] private Vector2 _targetScale = new Vector2(1.2f, 1.2f);
        [SerializeField] private float _fadeAndScaleDuration = 1f;

        [SerializeField] private float _animationDelay = 1f;

        private Sprite _defaultSprite;
        private Tween _currentTween;

        public void Play(string currentValue, string newValue)
        {
            _defaultSprite = _iconPlaceholder.sprite;
            _statFullValueLabel.text = currentValue;
        
            _itemTransform.localScale = _normalScale; 
            _iconPlaceholder.color = new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 1);
            _statFullValueLabel.color = new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 1);

            Sequence sequence = DOTween.Sequence();
            
            _currentTween = sequence;
            
            sequence.AppendInterval(_animationDelay);
            
            sequence.AppendCallback(() =>
            {
                _iconPlaceholder.sprite = _boostArrow;
            });
            
            sequence.AppendInterval(_spriteChangeDuration);

            sequence.AppendCallback(() =>
            {
                _itemTransform.localScale = _targetScale;
                _iconPlaceholder.sprite = _defaultSprite;
                _statFullValueLabel.text = newValue;
                _iconPlaceholder.color = new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 0);
                _statFullValueLabel.color = new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 0);
            });
            
            sequence.Append(_itemTransform.DOScale(_normalScale, _fadeAndScaleDuration));
            sequence.Join(_iconPlaceholder.DOColor(new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 1), _fadeAndScaleDuration / 2));
            sequence.Join(_statFullValueLabel.DOColor(new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 1), _fadeAndScaleDuration / 2));
        }
        
        public void Cleanup()
        {
            _currentTween?.Kill();
        }
    }
}