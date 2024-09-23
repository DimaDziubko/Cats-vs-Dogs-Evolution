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

        private Disposable<Shop> _screen;
        
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
            if (_screen != null) return _screen;
            _screen = await LoadDisposable<Shop>(AssetsConstants.SHOP);
            _screen.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _uiFactory,
                _shopPresenter,
                _checker,
                _logger);
            return _screen;
        }
        
        public override void Unload()
        {
            if (_screen != null)
            {
                _screen.Value.Hide();
                _screen.Dispose();
                _screen = null;
            }

            base.Unload();
        }
    }
}