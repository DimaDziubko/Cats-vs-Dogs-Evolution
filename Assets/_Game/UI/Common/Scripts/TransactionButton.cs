using System;
using System.Threading;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Assets._Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button), typeof(CustomButtonPressAnimator))]
    public class TransactionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<ButtonState> ButtonStateChanged;
        public event Action Click;
        public event Action InactiveClick;

        [SerializeField] private RectTransform _buttonRectTransform;
        [SerializeField] private TMP_Text _priceLabel;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private GameObject _moneyPanel;
        [SerializeField] private Image _currencyIconHolder;
        [SerializeField] private TMP_Text _infoLabel;
        [SerializeField] private Button _button;

        [SerializeField] private bool _isHoldable = false;
        
        private ButtonState _state = ButtonState.Inactive;

        private readonly Color _affordableColor = new Color(1f, 1f, 1f);

        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private bool _isPointerDown;
        private float _initialDelay = 0.5f;
        private float _repeatRate = 0.05f;

        private CancellationTokenSource _cancellationTokenSource;
        public RectTransform ButtonRectTransform => _buttonRectTransform;

        public void SetCurrencyIcon(Sprite currencyIcon)
        {
            _currencyIconHolder.gameObject.SetActive(true);
            
            if (_currencyIconHolder != null && currencyIcon != null)
            {
                _currencyIconHolder.sprite = currencyIcon;
                return;
            }
            _currencyIconHolder.gameObject.SetActive(false);
        }
        
        public void Init()
        {
            Unsubscribe();
            Subscribe();
        }

        public void UpdateButtonState(
            TransactionButtonModel model)
        {
            UpdateButtonState(model.State, model.Price, model.Info, model.ShowMoneyPanel);
        }

        public void UpdateButtonState(
            ButtonState state, 
            string price, 
            string info = null, 
            bool showInfo = false,
            bool showMoneyPanel = true)
        {
            switch (state)
            {
                case ButtonState.Active:
                    HandleActiveState(price, showInfo, showMoneyPanel);
                    break;
                case ButtonState.Inactive:
                    HandleInactiveState(price, showInfo, showMoneyPanel);
                    break;
                case ButtonState.Loading:
                    HandleLoadingState(showInfo);
                    break;
                case ButtonState.Recovering:
                    HandleRecoveringState(price,  info, showInfo, showMoneyPanel);
                    break;
            }
        }

        private void HandleRecoveringState(string price, string info, bool showInfo = true, bool showMoneyPanel = true)
        {
            _button.interactable = false;
            
            _infoLabel.gameObject.SetActive(showInfo);
            _infoLabel.text = info;
            _infoLabel.color = _affordableColor;
            
            _moneyPanel.SetActive(showMoneyPanel);
            _isPointerDown = false;
            
            if (_state != ButtonState.Recovering)
            {
                _state = ButtonState.Recovering;
                ButtonStateChanged?.Invoke(_state);
            }
            
            _priceLabel.text = price;
            _priceLabel.color = _expensiveColor;
            _infoText.color = _expensiveColor;
        }

        private void HandleActiveState(string price, bool showInfo = true, bool showMoneyPanel = true)
        {
            _button.interactable = true;
            _infoLabel.gameObject.SetActive(showInfo);
            _moneyPanel.SetActive(showMoneyPanel);
            if (_state != ButtonState.Active)
            {
                _state = ButtonState.Active;
                ButtonStateChanged?.Invoke(_state);
            }
            
            _priceLabel.text = price;
            _priceLabel.color = _affordableColor;
            _infoText.color = _affordableColor;
            
        }

        private void HandleInactiveState(string price, bool showInfo = true, bool showMoneyPanel = true)
        {
            _button.interactable = false;
            _infoLabel.gameObject.SetActive(showInfo);
            _moneyPanel.SetActive(showMoneyPanel);
            _isPointerDown = false;
            if (_state != ButtonState.Inactive)
            {
                _state = ButtonState.Inactive;
                ButtonStateChanged?.Invoke(_state);
            }
            _priceLabel.text = price;
            _priceLabel.color = _expensiveColor;
            _infoText.color = _expensiveColor;
        }

        private void HandleLoadingState(bool infoEnabled)
        {
            _button.interactable = false;
            _infoLabel.gameObject.SetActive(infoEnabled);
            _infoLabel.text = "Loading...";
            _moneyPanel.SetActive(false);
            _isPointerDown = false;
            if (_state != ButtonState.Loading)
            {
                _state = ButtonState.Loading;
                ButtonStateChanged?.Invoke(_state);
            }
        }

        public void Show() => 
            gameObject.SetActive(true);

        public void Hide() => 
            gameObject.SetActive(false);

        public void HideCurrencyIcon() => _currencyIconHolder.gameObject.SetActive(false);

        public void ShowCurrencyIcon() => _currencyIconHolder.gameObject.SetActive(true);

        public void Cleanup()
        {
            _isPointerDown = false;
            CancelAndDisposeCancellationToken();
            Unsubscribe();
        }

        private void Subscribe() => 
            _button.onClick.AddListener(OnTransitionButtonClick);

        private void Unsubscribe() => 
            _button.onClick.RemoveAllListeners();

        private void OnTransitionButtonClick()
        {
            Click?.Invoke();
        }

        private async UniTaskVoid ProcessHoldAction(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay), cancellationToken: cancellationToken);

                while (_isPointerDown && !cancellationToken.IsCancellationRequested)
                {
                    Click?.Invoke();
                    await UniTask.Delay(TimeSpan.FromSeconds(_repeatRate), cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_button == null)
                return;

            if (!_button.interactable)
            {
                InactiveClick?.Invoke();
                return;
            }
            
            if (_cancellationTokenSource != null)
            {
                CancelAndDisposeCancellationToken();
            }

            if (!_isHoldable) return;

            _isPointerDown = true;
            _cancellationTokenSource = new CancellationTokenSource();
            ProcessHoldAction(_cancellationTokenSource.Token).Forget();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
            CancelAndDisposeCancellationToken();
        }

        private void CancelAndDisposeCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}