using System;
using _Game.Core._Logger;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts
{
    public class FreeGemsPackPresenter : IProductPresenter, IInitializable, IDisposable
    {
        public FreeGemsPackView  View => _view;
        
        private FreeGemsPack _gemsPack;
        private FreeGemsPackView  _view;

        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly IFreeGemsPackService _freeGemsPackService;
        private readonly IMyLogger _logger;

        public FreeGemsPackPresenter(
            FreeGemsPack gemsPack,
            FreeGemsPackView view,
            IAssetProvider assetProvider,
            IAudioService audioService,
            IFreeGemsPackService freeGemsPackService,
            IMyLogger logger)
        {
            _gemsPack = gemsPack;
            _view = view;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _freeGemsPackService = freeGemsPackService;
            _logger = logger;
        }

        public async void Initialize()
        {
            OnAmountChanged(_gemsPack.Amount);
            
            Sprite majorIcon = await _assetProvider.Load<Sprite>(_gemsPack.Config.MajorIconKey);
            _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = await _assetProvider.Load<Sprite>(_gemsPack.Config.MinorIconKey);
            _view.SetMinorIcon(minorIcon);
            
            _view.Button.HideCurrencyIcon();

            _view.SetQuantity(_gemsPack.Config.Quantity.ToString());
            
            HandleViewStates();

            _view.Button.Click += OnBuyButtonClicked;
            _gemsPack.TimeChanged += OnTimerChanged;
            _gemsPack.IsReadyChanged += OnReadyChanged;
            _gemsPack.AmountChanged += OnAmountChanged;
        }

        private void HandleViewStates()
        {
            if (!_gemsPack.IsReady) HandleRecoveringState();
            else HandleActiveState();
        }

        public void Dispose()
        {
            _view.Button.Click -= OnBuyButtonClicked;
            _gemsPack.TimeChanged -= OnTimerChanged;
            _gemsPack.IsReadyChanged -= OnReadyChanged;
            _gemsPack.AmountChanged -= OnAmountChanged;
        }

        private void OnAmountChanged(int amount)
        {
            string info = _gemsPack.Amount <= 1 ? "Free" : _gemsPack.Info;
            _view.Button.UpdateButtonState(ButtonState.Active, info);
        }

        private void OnTimerChanged(float remainingTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            _view.UpdateTimer($"{timeSpan.Minutes}m:{timeSpan.Seconds}s");
        }

        private void HandleActiveState()
        {
            _view.SetActiveTimerView(false);
            _view.Button.Show();
        }

        private void HandleRecoveringState()
        {
            _view.SetActiveTimerView(true);
            _view.Button.Hide();
        }

        private void OnReadyChanged(bool isReady) => HandleViewStates();

        private void OnBuyButtonClicked()
        {
            _freeGemsPackService.OnFreeGemsPackBtnClicked(_gemsPack);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter>
        {
            
        }
    }
}