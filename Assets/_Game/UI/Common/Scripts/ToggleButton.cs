using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Image _changableImage;

        [SerializeField] private Sprite _icon;
        [SerializeField] private Sprite _lock;
        [SerializeField] private Image _iconHolder;
        
        //Animation
        [SerializeField] private RectTransform _iconTransform;
        [SerializeField] private float _iconWarp = 0.1f;
        [SerializeField] private Vector3 _normalScale = Vector3.one;
        [SerializeField] private Vector3 _highlightedScale = new Vector3(1f, 1.1f, 1f);

        private Vector3 _normalIconPosition;
        private Vector3 _highlightedIconPosition;

        public void Initialize(bool isLocked, Action<ToggleButton> callback, Action playSound)
        {
            if (_iconTransform != null)
            {
                _normalIconPosition = _iconTransform.position;
                _highlightedIconPosition = new Vector3(
                    _normalIconPosition.x, 
                    _normalIconPosition.y + _iconWarp, 
                    0);
            }
            
            if (isLocked)
            {
                Lock();
                return;
            }

            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                callback?.Invoke(this);
                playSound?.Invoke();
            });
            Unlock();
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void Lock()
        {
            _button.interactable = false;
            if (_iconHolder != null && _lock != null)
            {
                _iconHolder.sprite = _lock;
            }
        }

        private void Unlock()
        {
            _button.interactable = true;
            if (_iconHolder != null && _lock != null)
            {
                _iconHolder.sprite = _icon;
            }
        }
        
        public void HighlightBtn()
        {
            if (_activeSprite != null && _changableImage != null)
            {
                _changableImage.sprite = _activeSprite;
            }

            if (_iconTransform)
            {
                _iconTransform.position = _highlightedIconPosition;
            }

            _button.transform.localScale = _highlightedScale;
        }

        public void UnHighlightBtn()
        {
            if (_inactiveSprite != null && _changableImage != null)
            {
                _changableImage.sprite = _inactiveSprite;
            }

            if (_iconTransform)
            {
                _iconTransform.position = _normalIconPosition;
            }
            
            _button.transform.localScale = _normalScale;
        }
        
    }
}
