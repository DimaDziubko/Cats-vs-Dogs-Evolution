using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Settings.Scripts
{
    public class SettingsPopupProvider : LocalAssetLoader, ISettingsPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        public SettingsPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }
        public async UniTask<Disposable<SettingsPopup>> Load()
        {
            var popup = await LoadDisposable<SettingsPopup>(AssetsConstants.SETTINGS);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService);
            return popup;
        }
    }
}