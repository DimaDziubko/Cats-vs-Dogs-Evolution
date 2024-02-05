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

        public void Initialize(Action<ToggleButton> callback)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => callback?.Invoke(this));
        }
    
        public void HighlightBtn()
        {
            //TODO Change sprite
            
            Vector3 targetScale = new Vector3(1, 1.1f, 1);
            _button.transform.localScale = targetScale;
        }
        
        public void UnHighlightBtn()
        {
            //TODO Change sprite
            
            Vector3 targetScale = new Vector3(1, 1, 1);
            _button.transform.localScale = targetScale;
        }

        private void OnDisable()
        {
            //TODO Delete
            Debug.Log("Toggle button unsubscribed");
            _button.onClick.RemoveAllListeners();
        }
    }
}
