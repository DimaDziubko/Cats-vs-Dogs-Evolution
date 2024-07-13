using _Game.Core.AssetManagement;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineInfoWindowProvider : LocalAssetLoader, ITimelineInfoWindowProvider
    {
        private readonly IAudioService _audioService;
        private readonly ITimelineInfoPresenter _timelineInfoPresenter;
        private readonly IEvolutionPresenter _evolutionPresenter;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;

        public TimelineInfoWindowProvider(
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IEvolutionPresenter evolutionPresenter, 
            IMyLogger logger,
            IWorldCameraService cameraService)
        {
            _audioService = audioService;
            _timelineInfoPresenter = timelineInfoPresenter;
            _evolutionPresenter = evolutionPresenter;
            _logger = logger;
            _cameraService = cameraService;
        }
        public async UniTask<Disposable<global::_Game.UI.TimelineInfoWindow.Scripts.TimelineInfoWindow>> Load()
        {
            var window = await
                LoadDisposable<global::_Game.UI.TimelineInfoWindow.Scripts.TimelineInfoWindow>(AssetsConstants.TIMELINE_INFO_WINDOW);
            
            window.Value.Construct(
                _audioService,
                _timelineInfoPresenter,
                _evolutionPresenter,
                _logger,
                _cameraService);
            return window;
        }
    }
}