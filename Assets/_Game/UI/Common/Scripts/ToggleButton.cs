using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.UI.Pin.Scripts;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(
        typeof(Button),
        typeof(ToggleButtonStateAnimator),
        typeof(ToggleButtonView))]
    public class ToggleButton : MonoBehaviour, IFeature
    {
        public event Action<ButtonState> ButtonStateChanged;

        [SerializeField] private Feature _feature;
        
        [SerializeField] private Button _button;
        [SerializeField] private PinView _pin;
        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private RectTransform _iconTransform;
        
        [SerializeField] private ToggleButtonStateAnimator _animator;
        [SerializeField] private ToggleButtonView _view;
        
        public RectTransform RectTransform => _buttonTransform;
        public Feature Feature => _feature;

        private ButtonState _state;
        
        public void Initialize(bool isUnlocked, 
            Action<ToggleButton> callback,
            Action playSound, 
            NotificationData data = null)
        {
            HidePin();
            if(isUnlocked && data != null) SetupPin(data);

            var newState = isUnlocked ? ButtonState.Active : ButtonState.Inactive;
            if (_state != newState)
            {
                _state = newState;
                ButtonStateChanged?.Invoke(_state);
            }
            
            _animator.Initialize(
                _buttonTransform, 
                _iconTransform);
            SetupButton(isUnlocked, callback, playSound);
        }
        
        private void SetupButton(bool isUnlocked, Action<ToggleButton> callback, Action playSound)
        {
            if (!isUnlocked)
            {
                Lock();
                return;
            }

            Unlock();
            
            _button.onClick.AddListener(() =>
            {
                playSound?.Invoke();
                callback?.Invoke(this);
            });
        }

        public void SetupPin(NotificationData data)
        {
            if(_state == ButtonState.Inactive) return; 
            if (data.IsReviewed || !data.IsAvailable)
            {
                HidePin(); 
            }
            else
            {
                ShowPin(); 
            }
        }

        private void ShowPin() => _pin.Show();

        private void HidePin() => _pin.Hide();

        public void Cleanup() => 
            _button.onClick.RemoveAllListeners();

        private void Lock()
        {
            SetInteractable(false);
            _view.SetIcon(true);
            _view.SetText(true);
        }

        private void Unlock()
        {
            SetInteractable(true);
            _view.SetIcon(false);
            _view.SetText(false);
        }

        private void SetInteractable(bool isInteractable) => 
            _button.interactable = isInteractable;

        public void HighlightBtn()
        {
            _view.Highlight();
            _animator.Highlight();
        }

        public void UnHighlightBtn()
        {
            _view.UnHighlight();
            _animator.UnHighlight();
        }
    }
}
