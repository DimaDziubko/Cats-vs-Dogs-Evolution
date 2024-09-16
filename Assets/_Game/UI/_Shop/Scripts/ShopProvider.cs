using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop.Scripts
{
    public class ShopProvider : LocalAssetLoader, IShopProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IUIFactory _uiFactory;
        private readonly IShopPresenter _shopPresenter;
        private readonly IUpgradesAvailabilityChecker _checker;
        private readonly IMyLogger _logger;

        public ShopProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IUIFactory uiFactory,
            IShopPresenter shopPresenter,
            IUpgradesAvailabilityChecker checker, 
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _uiFactory = uiFactory;
            _shopPresenter = shopPresenter;
            _checker = checker;
            _logger = logger;
        }
        public async UniTask<Disposable<Shop>> Load()
        {
            var popup = await LoadDisposable<Shop>(AssetsConstants.SHOP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _uiFactory,
                _shopPresenter,
                _checker,
                _logger);
            return popup;
        }
    }
}