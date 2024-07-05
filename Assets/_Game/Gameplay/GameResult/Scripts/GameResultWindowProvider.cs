using Assets._Game.Core._Logger;
using Assets._Game.Core.Ads;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Gameplay.GameResult.Scripts
{
    public class GameResultWindowProvider : LocalAssetLoader, IGameResultWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IAdsService _adsService;
        private readonly IMyLogger _logger;

        public GameResultWindowProvider(
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
        public async UniTask<Disposable<GameResultWindow>> Load()
        {
            var window = await
                LoadDisposable<GameResultWindow>(AssetsConstants.GAME_RESULT_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _logger,
                _adsService);
            return window;
        }
    }
}