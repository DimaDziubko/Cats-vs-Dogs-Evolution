using System;
using System.Globalization;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Temp;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using _Game.UI.Common.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._GemsBundle
{
    public class GemsBundlePresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;
        
        private GemsBundle _bundle;
        private GemsBundleView _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;

        public GemsBundlePresenter(
            GemsBundle bundle, 
            GemsBundleView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IIAPService iapService)
        {
            _bundle = bundle;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _iapService = iapService;
        }


        public async void Initialize()
        {
            _view.SetQuantity(_bundle.Config.Quantity.ToString(CultureInfo.InvariantCulture));
            
            Sprite majorIcon = await _assetProvider.Load<Sprite>(_bundle.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = await _assetProvider.Load<Sprite>(_bundle.Config.MinorIconKey);
            _view.SetMinorIcon(minorIcon);
            
            _view.Button.UpdateButtonState(ButtonState.Active, _bundle.Product.metadata.localizedPriceString);
            _view.Button.HideCurrencyIcon();
            
            _view.Button.Click += OnBuyButtonClicked;
            _view.Button.InactiveClick += OnInactiveButtonClicked;
        }

        public void Dispose()
        {
            _view.Button.Click -= OnBuyButtonClicked;
            _view.Button.InactiveClick -= OnInactiveButtonClicked;
        }
        
        private void OnInactiveButtonClicked() => 
            GlobalEvents.RaiseOnInsufficientFunds();

        private void OnBuyButtonClicked()
        {
            _iapService.StartPurchase(_bundle.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<GemsBundle, GemsBundleView, GemsBundlePresenter>
        {
            
        }
    }
}