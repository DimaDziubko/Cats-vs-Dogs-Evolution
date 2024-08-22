using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.UI._Hud._DailyTaskView
{
    public class DailyTaskViewAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewTransform;
        [SerializeField] private float _normalScale = 1;
        [SerializeField] private float _maxScale = 1.2f;
        [SerializeField] private float _notificationDelay = 10f;
        [SerializeField] private float _notificationTime = 1f;

        [SerializeField] private float _refreshTime = 2f;
    
        [SerializeField] private bool _isNotificationAnimationActive = true;
        [SerializeField] private bool _isRefreshAnimationActive = true;

        private bool _isNotificationPlaying = false;

        private Tween _notificationTween;
        public void PlayRefreshAnimation(Action callback)
        {
            if(!_isRefreshAnimationActive) return;

            _viewTransform.DOScale(0, _refreshTime / 2).OnComplete(() =>
            {
                callback?.Invoke();
                _viewTransform.DOScale(_normalScale, _refreshTime / 2);
            });
        }

        public void PlayNotificationAnimation()
        {
            if(!_isNotificationAnimationActive || _isNotificationPlaying) return;
    
            StopNotificationAnimation();

            _isNotificationPlaying = true;
            
            _notificationTween = DOTween.Sequence()
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .AppendInterval(_notificationDelay)
                .SetLoops(-1, LoopType.Restart)
                .OnKill(() => _isNotificationPlaying = false);
        }

        public void StopNotificationAnimation()
        {
            if (_notificationTween != null && _notificationTween.IsActive())
            {
                _notificationTween.Kill();
                _notificationTween = null;
            }

            _viewTransform.localScale = Vector3.one * _normalScale;
            _isNotificationPlaying = false;
        }
        
        public void PlayAppearAnimation(Action callback)
        {
            _viewTransform.localScale = Vector3.zero;
            _viewTransform.DOScale(_normalScale, _refreshTime).OnComplete(() => callback?.Invoke());
        }

        public void PlayDisappearAnimation(Action callback)
        {
            _viewTransform.DOScale(0, _refreshTime).OnComplete(() => callback?.Invoke());
        }
    }
}
