using _Game.Core.Services.Camera;
using _Game.Core.UserState._State;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;

namespace _Game.UI._Currencies
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

        private double _currentCoinsQuantity;
        private double _currentGemsQuantity;
        
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
            
            OnCurrenciesChanged(Currencies.Coins, 0, CurrenciesSource.None);
            OnCurrenciesChanged(Currencies.Gems, 0, CurrenciesSource.None);
        }

        private void OnCurrenciesChanged(Currencies type, double delta, CurrenciesSource source)
        {
            switch (type)
            {
                case Currencies.Coins:
                    HandleCoins(source);
                    break;
                case Currencies.Gems:
                    HandleGems(source);
                    break;
            }
            
        }

        private void HandleGems(CurrenciesSource source)
        {
            switch (source)
            {
                default:
                    UpdateWithoutAnimation(ref _currentGemsQuantity, _currencies.Gems, _gemsLabel);
                    break;
            }
        }

        private void HandleCoins(CurrenciesSource source)
        {
            switch (source)
            {
                case CurrenciesSource.Battle:
                    UpdateWithAnimation(_coinsLabel, ref _currentCoinsQuantity, _currencies.Coins);
                    break;
                case CurrenciesSource.MiniShop:
                    UpdateWithAnimation(_coinsLabel, ref _currentCoinsQuantity, _currencies.Coins);
                    break;
                default:
                    UpdateWithoutAnimation(ref _currentCoinsQuantity, _currencies.Coins, _coinsLabel);
                    break;
            }
        }

        private void UpdateWithAnimation(TMP_Text label, ref double currentValue, double newValue)
        {
            _animator.PlayScaleAnimation(label);
            _animator.AnimateCurrenciesTextDelayed(label, currentValue, newValue);
            currentValue = newValue;
        }

        private void UpdateWithoutAnimation(ref double currentValue, double newValue, TMP_Text label)
        {
            currentValue = newValue;
            label.text = newValue.FormatMoney();
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