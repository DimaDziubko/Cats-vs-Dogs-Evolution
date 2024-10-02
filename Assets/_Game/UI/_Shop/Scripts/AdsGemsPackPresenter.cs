using System;
using _Game.Core._Logger;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts
{
    public class AdsGemsPackPresenter : IProductPresenter, IInitializable, IDisposable
    {
        public AdsGemsPackView  View => _view;
        
        private AdsGemsPack _gemsPack;
        private AdsGemsPackView  _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IAdsGemsPackService _adsGemsPackService;
        private readonly IMyLogger _logger;

        public AdsGemsPackPresenter(
            AdsGemsPack gemsPack,
            AdsGemsPackView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IAdsGemsPackService adsGemsPackService,
            IMyLogger logger)
        {
            _gemsPack = gemsPack;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _adsGemsPackService = adsGemsPackService;
            _logger = logger;
        }

        public async void Initialize()
        {
            OnAmountChanged(_gemsPack.Amount);
            
            Sprite majorIcon = await _assetProvider.Load<Sprite>(_gemsPack.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = await _assetProvider.Load<Sprite>(_gemsPack.Config.MinorIconKey);
            _view.SetMinorIcon(minorIcon);
            
            Sprite adIcon = await _assetProvider.Load<Sprite>(_gemsPack.Config.AdIconKey);
            _view.Button.SetCurrencyIcon(adIcon);
            _view.Button.ShowCurrencyIcon();

            _view.SetQuantity(_gemsPack.Config.Quantity.ToString());
            
            HandleViewStates();

            _view.Button.Click += OnBuyButtonClicked;
            _gemsPack.TimeChanged += OnTimerChanged;
            _gemsPack.IsLoadedChanged += OnLoaded;
            _gemsPack.IsReadyChanged += OnReadyChanged;
            _gemsPack.AmountChanged += OnAmountChanged;
        }

        private void HandleViewStates()
        {
            if (!_gemsPack.IsLoaded) HandleLoadingState();
            else if (!_gemsPack.IsReady) HandleRecoveringState();
            else HandleActiveState();
        }

        public void Dispose()
        {
            _view.Button.Click -= OnBuyButtonClicked;
            _gemsPack.TimeChanged -= OnTimerChanged;
            _gemsPack.IsLoadedChanged -= OnLoaded;
            _gemsPack.IsReadyChanged -= OnReadyChanged;
            _gemsPack.AmountChanged -= OnAmountChanged;
        }

        private void OnAmountChanged(int obj) => 
            _view.Button.UpdateButtonState(ButtonState.Active, _gemsPack.Info);

        private void OnTimerChanged(float remainingTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            _view.UpdateTimer($"{timeSpan.Minutes}m:{timeSpan.Seconds}s");
        }

        private void HandleActiveState()
        {
            _view.SetActiveTimerView(false);
            _view.SetActiveLoadingView(false);
            _view.Button.Show();
        }

        private void HandleRecoveringState()
        {
            _view.SetActiveTimerView(true);
            _view.SetActiveLoadingView(false);
            _view.Button.Hide();
        }

        private void HandleLoadingState()
        {
            _view.SetActiveTimerView(false);
            _view.SetActiveLoadingView(true);
            _view.SetLoadingText("Loading...");
            _view.Button.Hide();
        }

        private void OnReadyChanged(bool isReady) => HandleViewStates();
        private void OnLoaded(bool isLoaded) => HandleViewStates();

        private void OnBuyButtonClicked()
        {
            _adsGemsPackService.OnAdsGemsPackBtnClicked(_gemsPack);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter>
        {
            
        }
    }
}