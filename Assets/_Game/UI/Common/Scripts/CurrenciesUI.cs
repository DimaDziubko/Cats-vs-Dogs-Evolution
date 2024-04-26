using _Game.Core.Services.Camera;
using _Game.Core.UserState;
using _Game.Utils.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    public class CurrenciesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private RectTransform _coinsWalletTransform;
        
        [SerializeField] private float _scaleAnimationDuration = 0.1f;
        [SerializeField] private float _targetScale = 1.1f;
        [SerializeField] private float _normalScale = 1.0f;
        
        
        private IUserCurrenciesStateReadonly _currencies;
        private IWorldCameraService _cameraService;

        public Vector3 CoinsWalletWorldPosition => CalculateWorldPosition(_coinsWalletTransform);

        public void Construct(
            IUserCurrenciesStateReadonly  currencies,
            IWorldCameraService cameraService)
        {
            _currencies = currencies;
            _cameraService = cameraService;
        }

        public void Show()
        {
            _currencies.CoinsChanged -= OnCurrenciesChanged;
            _currencies.CoinsChanged += OnCurrenciesChanged;
            
            OnCurrenciesChanged(false);
        }

        private void OnCurrenciesChanged(bool isPositive)
        {
            if (isPositive)
            {
                PlayScaleAnimation();
                _coinsLabel.text = _currencies.Coins.FormatMoney();
            }
            else
            {
                _coinsLabel.text = _currencies.Coins.FormatMoney();
            }
        }

        private void PlayScaleAnimation()
        {
            _coinsLabel.transform.DOKill(); 
            _coinsLabel.transform.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_coinsLabel.transform.DOScale(_targetScale, _scaleAnimationDuration/2))
                .Append(_coinsLabel.transform.DOScale(_normalScale, _scaleAnimationDuration/2));
            sequence.Play();
        }
        
        public void Hide()
        {
            //TODO Fix later
            //_currencies.CoinsChanged -= OnCurrenciesChanged;
        }

        private Vector3 CalculateWorldPosition(RectTransform coinsWalletTransform)
        {
            Vector2 screenPoint = 
                RectTransformUtility.WorldToScreenPoint(
                    _cameraService.UICameraOverlay, 
                    _coinsWalletTransform.position);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                coinsWalletTransform,
                screenPoint, 
                _cameraService.UICameraOverlay, 
                out var worldPosition);

            return worldPosition;
        }
    }
}