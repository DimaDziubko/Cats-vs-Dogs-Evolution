using System;
using System.Globalization;
using _Game._AssetProvider;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.UI.Common.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts
{
    public class ProfitOfferPresenter : IProductPresenter, IInitializable, IDisposable
    {
        public ProfitOfferView  View => _view;
        
        private ProfitOffer _offer;
        private ProfitOfferView  _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;
        private readonly IMyLogger _logger;

        public ProfitOfferPresenter(
            ProfitOffer offer,
            ProfitOfferView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IIAPService iapService,
            IMyLogger logger)
        {
            _offer = offer;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _iapService = iapService;
            _logger = logger;
        }

        public async void Initialize()
        {
            Sprite majorIcon = await _assetProvider.Load<Sprite>(_offer.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = await _assetProvider.Load<Sprite>(_offer.Config.MinorIconKey);
            _view.SetMinorIcon(minorIcon);

            foreach (var moneyBox in _offer.Config.MoneyBoxes)
            {
                var element = _view.SpawnResourceElement();
                Sprite moneyIcon = await _assetProvider.Load<Sprite>(moneyBox.IconKey);
                element.SetIcon(moneyIcon);
                element.SetAmount(moneyBox.Quantity.ToString(CultureInfo.InvariantCulture));
                element.Show();
            }

            _view.SetDescription(_offer.Config.Description);
            _view.SetValue($"x{_offer.Config.CoinBoostFactor}");
            _view.SetName(_offer.Config.Name);
            _view.SetActive(_offer.IsActive);
            
            _view.Button.UpdateButtonState(ButtonState.Active, _offer.Product.metadata.localizedPriceString);
            _view.Button.HideCurrencyIcon();

            _offer.IsActiveChanged += OnActiveChanged;
            _view.Button.Click += OnBuyButtonClicked;
        }

        public void Dispose()
        {
            _offer.IsActiveChanged -= OnActiveChanged;
            _view.Button.Click -= OnBuyButtonClicked;
        }

        private void OnActiveChanged(bool isActive)
        {
            _view.SetActive(isActive, true);
        }

        private void OnBuyButtonClicked()
        {
            _iapService.StartPurchase(_offer.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<ProfitOffer, ProfitOfferView, ProfitOfferPresenter>
        {
            
        }
    }
}