using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.GameResult.Scripts
{
    public class GameResultPopupProvider : LocalAssetLoader, IGameResultPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IAdsService _adsService;
        private readonly IMyLogger _logger;

        public GameResultPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger,
            IAdsService adService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _adsService = adService;

            _logger = logger;
        }
        public async UniTask<Disposable<GameResultPopup>> Load()
        {
            var window = await
                LoadDisposable<GameResultPopup>(AssetsConstants.GAME_RESULT_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _logger,
                _adsService);
            return window;
        }
    }
}