using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.AssetManagement;
using _Game.Core.DataPresenters._UpgradeItemPresenter;
using _Game.Core.DataPresenters.Evolution;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Pin.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionWindowProvider : LocalAssetLoader, IUpgradeAndEvolutionWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private readonly IHeader _header;
        
        private readonly IUpgradeItemPresenter _upgradeItemPresenter;
        private readonly IEvolutionPresenter _evolutionPresenter;
        private readonly IUnitUpgradesPresenter _unitUpgradesPresenter;
        private readonly IMyLogger _logger;
        private readonly ITimelineInfoWindowProvider _timelineInfoWindowProvider;
        private readonly ITutorialManager _tutorialManager;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly ITimelineTravelPresenter _timelineTravelPresenter;

        public UpgradeAndEvolutionWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IUpgradeItemPresenter upgradeItemPresenter,
            IEvolutionPresenter evolutionPresenter,
            IUnitUpgradesPresenter unitUpgradesPresenter,
            IMyLogger logger,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IUpgradesAvailabilityChecker upgradesChecker, 
            ITimelineTravelPresenter timelineTravelPresenter)
        {
            _logger = logger;
            
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _upgradeItemPresenter = upgradeItemPresenter;
            _evolutionPresenter = evolutionPresenter;
            _unitUpgradesPresenter = unitUpgradesPresenter;
            _timelineInfoWindowProvider = timelineInfoWindowProvider;
            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;
            _upgradesChecker = upgradesChecker;
            _timelineTravelPresenter = timelineTravelPresenter;
        }
        public async UniTask<Disposable<UpgradeAndEvolutionWindow>> Load()
        {
            var popup = await LoadDisposable<UpgradeAndEvolutionWindow>(AssetsConstants.UPGRADE_AND_EVOLUTION_WINDOW);
            
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _upgradeItemPresenter,
                _evolutionPresenter,
                _timelineTravelPresenter,
                _unitUpgradesPresenter,
                _logger,
                _timelineInfoWindowProvider,
                _tutorialManager,
                _featureUnlockSystem,
                _upgradesChecker);
            return popup;
        }
    }
}
