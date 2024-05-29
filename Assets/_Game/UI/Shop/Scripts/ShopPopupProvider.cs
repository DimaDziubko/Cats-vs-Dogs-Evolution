using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Shop.Scripts
{
    public class ShopPopupProvider : LocalAssetLoader, IShopPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        public ShopPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }
        public async UniTask<Disposable<ShopPopup>> Load()
        {
            var popup = await LoadDisposable<ShopPopup>(AssetsConstants.SHOP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService);
            return popup;
        }
    }
}