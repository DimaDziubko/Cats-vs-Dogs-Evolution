using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay.GameResult.Scripts
{
    public class GameResultWindowProvider : LocalAssetLoader, IGameResultWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        
        private readonly IMyLogger _logger;

        public GameResultWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _audioService = audioService;

            _logger = logger;
        }
        public async UniTask<Disposable<GameResultWindow>> Load()
        {
            var window = await
                LoadDisposable<GameResultWindow>(AssetsConstants.GAME_RESULT_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _logger);
            return window;
        }
    }
}