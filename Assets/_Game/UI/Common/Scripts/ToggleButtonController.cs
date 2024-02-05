using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    public class ToggleButtonController : MonoBehaviour
    {
        [SerializeField] private ToggleButton[] _buttons;
        [SerializeField] private ToggleButton _defaultButton;
        
        private ToggleButton _activeButton;
        
        private void Awake()
        {
            foreach (var button in _buttons)
            {
                button.Initialize(ActivateButton);
            }

            HighlightDefaultButton();
        }

        private void HighlightDefaultButton()
        {
            _activeButton = _defaultButton;
            _activeButton.HighlightBtn();
        }

        private void ActivateButton(ToggleButton button)
        {
            if (_activeButton != null)
            {
                _activeButton.UnHighlightBtn();
            }
            
            _activeButton = button;
            _activeButton.HighlightBtn();
        }
    }
}
