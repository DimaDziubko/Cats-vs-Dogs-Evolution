using System;
using _Game._AssetProvider;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IGPService;
using _Game.Temp;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._CoinBundles
{
    public class CoinsBundlePresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;
        
        private readonly CoinsBundle _bundle;
        private readonly CoinsBundleView _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IIGPService _igpService;

        public CoinsBundlePresenter(
            CoinsBundle bundle, 
            CoinsBundleView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IIGPService igpService)
        {
            _bundle = bundle;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _igpService = igpService;
        }


        public async void Initialize()
        {
            OnQuantityChanged(_bundle.Quantity);
            
            Sprite currencyIcon = await _assetProvider.Load<Sprite>(_bundle.Config.CurrencyIconKey);
            _view.SetCurrencyIcon(currencyIcon);

            Sprite majorIcon = await _assetProvider.Load<Sprite>(_bundle.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = await _assetProvider.Load<Sprite>(_bundle.Config.MinorIconKey);
            _view.SetMinorIcon(minorIcon);

            OnAffordableChanged(_bundle.IsAffordable);
            
            _bundle.QuantityChanged += OnQuantityChanged;
            _bundle.IsAffordableChanged += OnAffordableChanged;
            
            _view.Button.Click += OnBuyButtonClicked;
            _view.Button.InactiveClick += OnInactiveButtonClicked;
        }

        private void OnAffordableChanged(bool isAffordable)
        {
            ButtonState buttonState =
                isAffordable ? ButtonState.Active : ButtonState.Inactive;
            
            _view.Button.UpdateButtonState(buttonState, _bundle.Config.Price.ToString());
        }

        public void Dispose()
        {
            _bundle.QuantityChanged -= OnQuantityChanged;
            _bundle.IsAffordableChanged -= OnAffordableChanged;
            
            _view.Button.Click -= OnBuyButtonClicked;
            _view.Button.InactiveClick -= OnInactiveButtonClicked;
        }

        private void OnQuantityChanged(float newValue) => 
            _view.SetQuantity(newValue.FormatMoney());

        private void OnInactiveButtonClicked() => 
            GlobalEvents.RaiseOnInsufficientFunds();

        private void OnBuyButtonClicked()
        {
            _igpService.StartPurchase(_bundle);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<CoinsBundle, CoinsBundleView, CoinsBundlePresenter>
        {
            
        }
    }
}