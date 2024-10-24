﻿using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.UserContainer;
using _Game.Temp;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShopProvider : LocalAssetLoader, IMiniShopProvider, IInitializable, IDisposable
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IUIFactory _uiFactory;
        private readonly IMiniShopPresenter _miniShopPresenter;
        private readonly IUpgradesAvailabilityChecker _checker;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        public bool IsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping);

        private Disposable<MiniShop> _miniShop;
        
        public MiniShopProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IUIFactory uiFactory,
            IMiniShopPresenter miniShopPresenter,
            IMyLogger logger, 
            IUserContainer userContainer,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _miniShopPresenter = miniShopPresenter;
            _logger = logger;
            _userContainer = userContainer;
            _featureUnlockSystem = featureUnlockSystem;
        }

        public async UniTask<Disposable<MiniShop>> Load()
        {
            _miniShop = await LoadDisposable<MiniShop>(AssetsConstants.MINI_SHOP);
            
            _miniShop.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _uiFactory,
                _miniShopPresenter,
                _logger,
                _userContainer);
            return _miniShop;
        }

        void IInitializable.Initialize() => 
            GlobalEvents.OnInsufficientFunds += OnInsufficientFunds;

        void IDisposable.Dispose()
        {
            GlobalEvents.OnInsufficientFunds -= OnInsufficientFunds;
            _miniShop?.Dispose();
        }

        private void OnInsufficientFunds() => _miniShop?.Value.ForceHide();
        
        public override void Unload()
        {
            if (_miniShop != null)
            {
                _miniShop.Value.Cleanup();
                _miniShop.Dispose();
                _miniShop = null;
            }
            
            base.Unload();
        }
    }
}