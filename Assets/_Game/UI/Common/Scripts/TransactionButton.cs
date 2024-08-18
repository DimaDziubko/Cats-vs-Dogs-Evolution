using System;
using System.Threading;
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
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private GameObject _moneyPanel;
        [SerializeField] private Image _currencyIconHolder;
        [SerializeField] private TMP_Text _loadingText;
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

        public void Construct(Sprite currencyIcon)
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
            ButtonState state, 
            string price, 
            bool showMoneyPanel = true)
        {
            switch (state)
            {
                case ButtonState.Active:
                    HandleActiveState(price, showMoneyPanel);
                    break;
                case ButtonState.Inactive:
                    HandleInactiveState(price, showMoneyPanel);
                    break;
                case ButtonState.Loading:
                    HandleLoadingState();
                    break;
            }
        }

        private void HandleActiveState(string price, bool showMoneyPanel = true)
        {
            _button.interactable = true;
            if(_loadingText != null)
                _loadingText.enabled = false;
            if (_moneyPanel != null) 
                _moneyPanel.SetActive(showMoneyPanel);
            if (_state != ButtonState.Active)
            {
                _state = ButtonState.Active;
                ButtonStateChanged?.Invoke(_state);
            }
            
            if (_priceText != null)
            {
                _priceText.text = price;
                _priceText.color = _affordableColor;
            }
            if (_infoText != null)
            {
                _infoText.color = _affordableColor;
            }
            
        }

        private void HandleInactiveState(string price, bool showMoneyPanel = true)
        {
            _button.interactable = false;
            if(_loadingText != null)
                _loadingText.enabled = false;
            if (_moneyPanel != null) 
                _moneyPanel.SetActive(showMoneyPanel);
            _isPointerDown = false;
            if (_state != ButtonState.Inactive)
            {
                _state = ButtonState.Inactive;
                ButtonStateChanged?.Invoke(_state);
            }
            
            if (_priceText != null)
            {
                _priceText.text = price;
                _priceText.color = _expensiveColor;
            }
            if (_infoText != null)
            {
                _infoText.color = _expensiveColor;
            }
            
        }

        private void HandleLoadingState()
        {
            _button.interactable = false;
            if(_loadingText != null)
                _loadingText.enabled = true;
            if (_moneyPanel != null) 
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

        public void Hide()
        {
            gameObject.SetActive(false);
            Cleanup();
        }

        public void Cleanup()
        {
            _isPointerDown = false;
            CancelAndDisposeCancellationToken();
            Unsubscribe();
        }

        private void Subscribe()
        {
            _button.onClick.AddListener(OnTransitionButtonClick);
        }

        private void Unsubscribe()
        {
            _button.onClick.RemoveAllListeners();
        }

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