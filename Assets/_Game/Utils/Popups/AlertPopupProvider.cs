using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Game.Utils.Popups
{
    public class AlertPopupProvider : LocalAssetLoader, IAlertPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        
        public AlertPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }
        public async UniTask<Disposable<AlertPopup>> Load()
        {
            var popup = await LoadDisposable<AlertPopup>(AssetsConstants.ALERT_POPUP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService);
            return popup;
        }
    }
}