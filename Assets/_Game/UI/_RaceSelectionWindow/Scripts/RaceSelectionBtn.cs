using System;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI._RaceSelectionWindow.Scripts
{
    [RequireComponent(typeof(Button))]
    public class RaceSelectionBtn : MonoBehaviour
    {
        [SerializeField] private Sprite _offSprite;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Image _changeableImage;
        [SerializeField] private Button _button;

        public bool IsOn { get; private set; }

        private Action<Race> _callback;
        private Race _race;
    
        public void Init(Race race, Action<Race> callback, bool isOn)
        {
            IsOn = isOn;
            _race = race;
            _callback = callback;
            _button.onClick.AddListener(OnButtonClicked);
            UpdateVisual();
        }

        private void OnButtonClicked()
        {
            IsOn = true;
            UpdateVisual();
            _callback?.Invoke(_race);
        }

        private void UpdateVisual()
        {
            _changeableImage.sprite = IsOn ? _onSprite : _offSprite;
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
            UpdateVisual();
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
