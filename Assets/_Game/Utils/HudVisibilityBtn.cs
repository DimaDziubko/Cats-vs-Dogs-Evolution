using _Game.Core._GameMode;
using _Game.UI.Header.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils
{
    public class HudVisibilityBtn : MonoBehaviour
    {
        [SerializeField] private GameObject[] _panels;
        [SerializeField] private Button _button;

        //Graphics
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        
        private IHeader _header;

        public void Construct(IHeader header)
        {
            _header = header;
            gameObject.SetActive(GameMode.I.IsCheatEnabled);
        }

        public void Init()
        {
            _button.onClick.AddListener(OnVisibilityBtnClicked);
        }

        private void OnVisibilityBtnClicked()
        {
            var isTextActive = _text.enabled;
            _text.enabled = !isTextActive;
            
            float newAlpha = _image.color.a == 1 ? 0 : 1;
            
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, newAlpha);
            
            foreach (var panel in _panels)
            {
                var isActive = panel.activeInHierarchy;
                panel.SetActive(!isActive);
                _header.SetActive(!isActive);
            }
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
