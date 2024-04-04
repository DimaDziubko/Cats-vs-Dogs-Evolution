using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineInfoWindowProvider : LocalAssetLoader, ITimelineInfoWindowProvider
    {
        private readonly IAudioService _audioService;
        
        private readonly IEvolutionService _evolutionService;
        
        private readonly IMyLogger _logger;

        public TimelineInfoWindowProvider(
            IAudioService audioService,
            IEvolutionService evolutionService,
            IMyLogger logger)
        {
            _audioService = audioService;
            _evolutionService = evolutionService;
            _logger = logger;
        }
        public async UniTask<Disposable<TimelineInfoWindow>> Load()
        {
            var window = await
                LoadDisposable<TimelineInfoWindow>(AssetsConstants.TIMELINE_INFO_WINDOW);
            
            window.Value.Construct(
                _audioService,
                _evolutionService,
                _logger);
            return window;
        }
    }
}