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

namespace _Game.UI._Shop.Scripts._SpeedOffer
{
    public class SpeedOfferPresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;
        
        private readonly SpeedOffer _offer;
        private readonly SpeedOfferView  _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;

        public SpeedOfferPresenter(
            SpeedOffer offer, 
            SpeedOfferView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IIAPService iapService)
        {
            _offer = offer;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _iapService = iapService;
        }


        public async void Initialize()
        {
            Sprite majorIcon = await _assetProvider.Load<Sprite>(_offer.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            _view.SetDescription(_offer.Config.Description);
            _view.SetValue($"x{_offer.Config.BattleSpeed.SpeedFactor.ToString(CultureInfo.InvariantCulture)}");

            _view.Button.UpdateButtonState(ButtonState.Active, _offer.Product.metadata.localizedPriceString);
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
            _iapService.StartPurchase(_offer.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<SpeedOffer, SpeedOfferView, SpeedOfferPresenter>
        {
            
        }
    }
}