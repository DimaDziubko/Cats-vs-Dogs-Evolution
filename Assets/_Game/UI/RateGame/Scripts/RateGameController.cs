using _Game.Core.AssetManagement;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.RateGame.Scripts
{
    public class RateGameController : LocalAssetLoader
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private readonly IMyLogger _logger;


        public RateGameController(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;

            _cameraService = cameraService;
            _audioService = audioService;

        }

        public async UniTask<Disposable<RateGameWindow>> Load()
        {
            var popup = await LoadDisposable<RateGameWindow>(AssetsConstants.RATE_GAME_WINDOW);

            popup.Value.Construct(
                _cameraService,
                _audioService,
                _logger
                );
            return popup;
        }
    }
}