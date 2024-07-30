using _Game.Core.Ads;
using _Game.Core.AssetManagement;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineInfoScreenProvider : LocalAssetLoader, ITimelineInfoScreenProvider
    {
        private readonly IAudioService _audioService;
        private readonly ITimelineInfoPresenter _timelineInfoPresenter;
        private readonly IEvolutionPresenter _evolutionPresenter;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;
        private readonly IAdsService _adsService;

        public TimelineInfoScreenProvider(
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IEvolutionPresenter evolutionPresenter,
            IMyLogger logger,
            IWorldCameraService cameraService, 
            IAdsService adsService)
        {
            _audioService = audioService;
            _timelineInfoPresenter = timelineInfoPresenter;
            _evolutionPresenter = evolutionPresenter;
            _logger = logger;
            _cameraService = cameraService;
            _adsService = adsService;
        }
        public async UniTask<Disposable<TimelineInfoScreen>> Load()
        {
            var window = await
                LoadDisposable<TimelineInfoScreen>(AssetsConstants.TIMELINE_INFO_SCREEN);
            
            window.Value.Construct(
                _audioService,
                _timelineInfoPresenter,
                _evolutionPresenter,
                _logger,
                _cameraService,
                _adsService);
            return window;
        }
    }
}