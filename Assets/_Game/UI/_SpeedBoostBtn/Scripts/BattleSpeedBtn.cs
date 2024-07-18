using System;
using Assets._Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI._SpeedBoostBtn.Scripts
{
    public enum BattleSpeedBtnState
    {
        Active,
        Inactive,
        Activated
    }

    public class BattleSpeedBtnModel
    {
        public BattleSpeedBtnState State;
        public string InfoText;
        public float TimerTime;
        public bool IsUnlocked;
    }

    [RequireComponent(typeof(Button))]
    public class BattleSpeedBtn : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Image _changeableImage;
        [SerializeField] private Image _adsIcon;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Sprite _activatedSprite;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _loadingText;

        [SerializeField] private Button _button;

        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private float _normalSizeX = 250f;
        [SerializeField] private float _normalSizeY = 60f;
        [SerializeField] private float _activatedSizeX = 170f;
        [SerializeField] private float _activatedSizeY = 120f;
        [SerializeField] private float _timerColorTreshold = 5f;

        private BattleSpeedBtnState State { get; set; }


        public void Initialize(Action<BattleSpeedBtnState> callback)
        {
            _button.onClick.AddListener(() =>
            {
                callback?.Invoke(State);
            });
        }

        public void UpdateBtnState(BattleSpeedBtnModel model)
        {
            switch (model.State)
            {
                case BattleSpeedBtnState.Active:
                    State = BattleSpeedBtnState.Active;
                    HandleActiveState(model);
                    break;
                case BattleSpeedBtnState.Inactive:
                    State = BattleSpeedBtnState.Inactive;
                    HandleInactiveState(model);
                    break;
                case BattleSpeedBtnState.Activated:
                    State = BattleSpeedBtnState.Activated;
                    HandleActivatedState(model);
                    break;
            }

            gameObject.SetActive(model.IsUnlocked);
        }

        private void HandleActiveState(BattleSpeedBtnModel model)
        {
            _changeableImage.sprite = _activeSprite;
            _panel.SetActive(true);
            _adsIcon.gameObject.SetActive(true);
            _infoText.text = model.InfoText;
            _timerText.enabled = false;
            _loadingText.enabled = false;
            _button.interactable = true;
            _buttonTransform.sizeDelta = new Vector2(_normalSizeX, _normalSizeY);
        }

        private void HandleInactiveState(BattleSpeedBtnModel model)
        {
            _changeableImage.sprite = _inactiveSprite;
            _panel.SetActive(false);
            _timerText.enabled = false;
            _loadingText.enabled = true;
            _button.interactable = false;
            _buttonTransform.sizeDelta = new Vector2(_normalSizeX, _normalSizeY);
        }

        private void HandleActivatedState(BattleSpeedBtnModel model)
        {
            _changeableImage.sprite = _activatedSprite;
            _panel.SetActive(true);
            _adsIcon.gameObject.SetActive(false);
            _infoText.text = model.InfoText;
            _timerText.enabled = true;
            UpdateTimer(model.TimerTime);
            _loadingText.enabled = false;
            _button.interactable = true;
            _buttonTransform.sizeDelta = new Vector2(_activatedSizeX, _activatedSizeY);
        }

        public void UpdateTimer(float timeLeft)
        {
            SetColor(timeLeft);
            _timerText.text = timeLeft.FormatTime();
        }

        private void SetColor(float timeLeft)
        {
            Color colorToSet = Color.green;

            if (timeLeft < _timerColorTreshold)
                colorToSet = Color.red;

            if (_timerText.color != colorToSet)
                _timerText.color = colorToSet;
        }
    }
}