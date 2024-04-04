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
        [SerializeField] private float _textIncreaseAnimationDuration = 1f;
        [SerializeField] private RectTransform _coinsWalletTransform;
        [SerializeField] private float _increaseAnimationDelay = 3f;
        
        private float _currentCoinsValue;
        
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
            _currentCoinsValue = _currencies.Coins;
            
            _currencies.CoinsChanged -= OnCurrenciesChanged;
            _currencies.CoinsChanged += OnCurrenciesChanged;
            
            OnCurrenciesChanged(false);
        }

        private void OnCurrenciesChanged(bool isPositive)
        {
            if (isPositive)
            {
                DOVirtual.DelayedCall(_increaseAnimationDelay, PlayIncreaseAnimation);
            }
            else
            {
                _coinsLabel.text = _currencies.Coins.FormatMoney();
            }
        }

        private void PlayIncreaseAnimation()
        {
            DOTween.To(
                () => _currentCoinsValue, 
                x => _currentCoinsValue = x, 
                _currencies.Coins,
                _textIncreaseAnimationDuration).OnUpdate(
                () =>
                {
                    _coinsLabel.text = _currentCoinsValue.FormatMoney();
                });
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