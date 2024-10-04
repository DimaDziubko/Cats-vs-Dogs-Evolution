using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI._RaceSelectionScreen.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Settings.Scripts
{
    public class SettingsPopupProvider : LocalAssetLoader, ISettingsPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IRaceSelectionWindowProvider _raceSelectionWindowProvider;

        public SettingsPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IRaceSelectionWindowProvider raceSelectionWindowProvider)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _raceSelectionWindowProvider = raceSelectionWindowProvider;
        }
        public async UniTask<Disposable<SettingsPopup>> Load()
        {
            var popup = await LoadDisposable<SettingsPopup>(AssetsConstants.SETTINGS);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _raceSelectionWindowProvider);
            return popup;
        }
    }
}