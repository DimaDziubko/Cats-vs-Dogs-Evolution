using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialStep : MonoBehaviour, ITutorialStep
    {
        public event Action<ITutorialStep> Show;
        public event Action<ITutorialStep> Complete;
        public event Action<ITutorialStep> Cancel;
        
        [SerializeField] private int _step;
        [SerializeField] private Vector2 _requiredPointerSize;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private RectTransform _rootCanvasTransform;
        [SerializeField] private RectTransform _tutorialObjectRectTransform;
        [SerializeField] private bool _isUnderneath;
        [SerializeField] private bool _needAppearanceAnimation;
        
        private TutorialStepData _data;

        public void ShowStep() => Show?.Invoke(this);

        public void CompleteStep() => Complete?.Invoke(this);

        public void CancelStep() => Cancel?.Invoke(this);
        
        public TutorialStepData GetTutorialStepData()
        {
            if (_data == null) SetupData();
            _data.RequiredPointerPosition = CalculateRequiredPointerPosition();
            _data.RequiredPointerRotation = CalculateRequiredRotation();
            return _data;
        }
        
        private void SetupData()
        {
            _data = new TutorialStepData()
            {
                Step = _step,
                RequiredPointerSize = _requiredPointerSize,
                RequiredPointerPosition = CalculateRequiredPointerPosition(),
                RequiredPointerRotation = CalculateRequiredRotation(),
                NeedAppearanceAnimation = _needAppearanceAnimation,
            };
        }

        private Vector3 CalculateRequiredPointerPosition()
        {
            var positionMultiplier = _isUnderneath ? -1 : 1;

            Canvas.ForceUpdateCanvases();
            
            Vector3 worldPosition = _tutorialObjectRectTransform.TransformPoint(_tutorialObjectRectTransform.rect.center);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvasTransform, worldPosition, null, out var canvasPosition);

            Vector3 requiredPointerPosition = new Vector3(
                canvasPosition.x  + _offset.x,
                canvasPosition.y + ((_requiredPointerSize.y  + _tutorialObjectRectTransform.sizeDelta.y / 2 + _offset.y)) * positionMultiplier,
                0);

            return requiredPointerPosition;
        }

        private Quaternion CalculateRequiredRotation()
        {
            Quaternion requiredPointerRotation = _isUnderneath ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            return requiredPointerRotation;
        }
    }
}