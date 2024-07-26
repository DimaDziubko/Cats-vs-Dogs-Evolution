using _Game.Core.UserState;
using _Game.Utils.Extensions;
using Assets._Game.Core.Services.Camera;
using TMPro;
using UnityEngine;

namespace _Game.UI.Currencies
{
    public enum Currencies
    {
        Coins,
        Gems
    }
    
    public class CurrenciesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private RectTransform _coinsWalletTransform;
        
        [SerializeField] private TMP_Text _gemsLabel;
        [SerializeField] private RectTransform _gemsWalletTransform;

        [SerializeField] private TextScaleAnimator _animator;

        private float _currentCoinsQuantity;
        private float _currentGemsQuantity;
        
        private IUserCurrenciesStateReadonly _currencies;
        private IWorldCameraService _cameraService;

        public Vector3 CoinsWalletWorldPosition => CalculateWorldPosition(_coinsWalletTransform);
        public Vector3 GemsWalletWorldPosition => CalculateWorldPosition(_gemsWalletTransform);

        public void Construct(
            IUserCurrenciesStateReadonly  currencies,
            IWorldCameraService cameraService)
        {
            _currencies = currencies;
            _cameraService = cameraService;
        }

        public void Show()
        {
            _currencies.CurrenciesChanged -= OnCurrenciesChanged;
            _currencies.CurrenciesChanged += OnCurrenciesChanged;
            
            OnCurrenciesChanged(Currencies.Coins, false);
            OnCurrenciesChanged(Currencies.Gems, false);
        }

        private void OnCurrenciesChanged(Currencies type, bool isPositive)
        {
            switch (type)
            {
                case Currencies.Coins:
                    HandleCoins(isPositive);
                    break;
                case Currencies.Gems:
                    HandleGems(isPositive);
                    break;
            }
            
        }

        private void HandleGems(bool isPositive)
        {
            if (isPositive)
            {
                _animator.PlayScaleAnimation(_gemsLabel);
                _animator.AnimateCurrenciesTextDelayed(_gemsLabel, _currentGemsQuantity, _currencies.Gems);
            }
            else
            {
                _currentGemsQuantity = _currencies.Gems;
                _gemsLabel.text = _currencies.Gems.FormatMoney();
            }
        }

        private void HandleCoins(bool isPositive)
        {
            if (isPositive)
            {
                _animator.PlayScaleAnimation(_coinsLabel);
                _animator.AnimateCurrenciesTextDelayed(_coinsLabel, _currentCoinsQuantity, _currencies.Coins);
            }
            else
            {
                _currentCoinsQuantity = _currencies.Coins;
                _coinsLabel.text = _currencies.Coins.FormatMoney();
            }
        }

        public void Hide()
        {
            
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