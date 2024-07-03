using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Utils.Popups
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