using DG.Tweening;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class ScaleAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Vector2 _startScale = new Vector2(0f, 0f);
        [SerializeField] private Vector2 _targetScale = new Vector2(1.2f, 1.2f);
        [SerializeField] private Vector2 _normalScale = new Vector2(1f, 1f);

        [SerializeField] private float _duration = 1f;

        public void Init()
        {
            _transform.localScale = _startScale;
        }

        public void Play()
        {
            _transform.localScale = _startScale;
            
            _transform.DOScale(_targetScale, _duration / 2)
                .OnComplete(() => _transform.DOScale(_normalScale, _duration / 2)); 
        }
    }
}