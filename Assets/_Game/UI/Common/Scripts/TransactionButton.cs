using System;
using System.Threading;
using _Game.Utils.Extensions;
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
        public event Action Click;

        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _infoText;

        [SerializeField] private bool _isHoldable;
        
        private Button _button;
        
        private readonly Color _affordableColor = new Color(1f, 1f, 1f);

        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private bool _isPointerDown;
        private float _initialDelay = 0.5f; 
        private float _repeatRate = 0.05f; 
        
        private CancellationTokenSource _cancellationTokenSource;
        
        public void Init()
        {
            _button = GetComponent<Button>();
            
            Unsubscribe();
            Subscribe();
        }

        public void UpdateButtonState(bool canAfford, float price)
        {
            _button.interactable = canAfford;

            if (!canAfford) _isPointerDown = false;
            
            if (_priceText != null)
            {
                _priceText.text = price.FormatMoney();
                _priceText.color = canAfford ? _affordableColor : _expensiveColor;
            }
            if (_infoText != null)
            {
                _infoText.color = canAfford ? _affordableColor : _expensiveColor;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

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
            if (!_button)
            {
                _button = GetComponent<Button>();
            }

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
            if (!_isHoldable || !_button.interactable) return;

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