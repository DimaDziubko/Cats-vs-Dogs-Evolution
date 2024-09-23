using _Game.UI.Factory;
using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialPointerView : MonoBehaviour
    {
        [SerializeField] private RectTransform _pointerTransform;
        [SerializeField] private RectTransform _arrowViewTransform;
        [SerializeField] private Animation _animation;

        //Appearance animation data
        [SerializeField] private Vector3 DefaultPosition = new Vector3(0, 1900, 0);
        [SerializeField] private Vector3 StartAppearanceScale = new Vector3(4, 4, 1);
        [SerializeField] private float _animationDuration = 0.5f;

        public IUIFactory OriginFactory { get; set; }

        private Tween _transformTween;
        private Tween _scaleTween;

        public void Show(TutorialStepData tutorialData)
        {
            if (tutorialData.NeedAppearanceAnimation)
            {
                Rotation = tutorialData.RequiredPointerRotation;
                ShowWithAppearanceAnimation(tutorialData);
                return;
            }
    
            Enable();
            Position = tutorialData.RequiredPointerPosition;
            Rotation = tutorialData.RequiredPointerRotation;
            Size = tutorialData.RequiredPointerSize;
            StartAnimation();
        }

        private void ShowWithAppearanceAnimation(TutorialStepData tutorialData)
        {
            _transformTween?.Kill();
            _scaleTween?.Kill();
            
            Enable();
            _pointerTransform.anchoredPosition = tutorialData.IsUnderneath ? 
                new Vector2(tutorialData.RequiredPointerPosition.x, -DefaultPosition.y) :
                new Vector2(tutorialData.RequiredPointerPosition.x, DefaultPosition.y);
            
            
            
            _pointerTransform.localScale = StartAppearanceScale;

            _transformTween = _pointerTransform.DOAnchorPosY(tutorialData.RequiredPointerPosition.y, _animationDuration)
                .SetEase(Ease.OutQuad);

            _scaleTween = _pointerTransform.DOScale(Vector3.one, _animationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    Position = tutorialData.RequiredPointerPosition;
                    Rotation = tutorialData.RequiredPointerRotation;
                    Size = tutorialData.RequiredPointerSize;
                    StartAnimation();
                });
        }

        public void Hide()
        {
            _scaleTween?.Kill();
            _transformTween?.Kill();
            StopAnimation();
            Disable();
            OriginFactory.Reclaim(this);
        }

        private Vector3 Position
        {
            get => _pointerTransform.anchoredPosition;
            set => _pointerTransform.anchoredPosition = value;
        }

        private Quaternion Rotation
        {
            get => _pointerTransform.rotation;
            set => _pointerTransform.rotation = value;
        }

        private Vector3 Size
        {
            get => _arrowViewTransform.sizeDelta;
            set => _arrowViewTransform.sizeDelta = new Vector2(value.x, value.y);
        }

        private void StartAnimation()
        {
            _animation.Play();
        }

        private void StopAnimation()
        {
            _animation.Stop();
        }

        private void Enable()
        {
            gameObject.SetActive(true);
        }
    
        private void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetActive(bool isVisible)
        {
            if (isVisible)
            {
                Enable();
                StartAnimation();
                return;
            }
            Disable();
        }
    }
}
