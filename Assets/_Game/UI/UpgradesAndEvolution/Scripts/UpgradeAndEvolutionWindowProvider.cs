using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
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
        
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IEvolutionService _evolutionService;
        private readonly IUnitUpgradesService _unitUpgradesService;
        private readonly IMyLogger _logger;
        private readonly ITimelineInfoWindowProvider _timelineInfoWindowProvider;
        private readonly ITutorialManager _tutorialManager;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;

        public UpgradeAndEvolutionWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IEconomyUpgradesService economyUpgradesService,
            IEvolutionService evolutionService,
            IUnitUpgradesService unitUpgradesService,
            IMyLogger logger,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _logger = logger;
            
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _economyUpgradesService = economyUpgradesService;
            _evolutionService = evolutionService;
            _unitUpgradesService = unitUpgradesService;
            _timelineInfoWindowProvider = timelineInfoWindowProvider;
            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;
            _upgradesChecker = upgradesChecker;
        }
        public async UniTask<Disposable<UpgradeAndEvolutionWindow>> Load()
        {
            var popup = await LoadDisposable<UpgradeAndEvolutionWindow>(AssetsConstants.UPGRADE_AND_EVOLUTION_WINDOW);
            
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _economyUpgradesService,
                _evolutionService,
                _unitUpgradesService,
                _logger,
                _timelineInfoWindowProvider,
                _tutorialManager,
                _featureUnlockSystem,
                _upgradesChecker);
            return popup;
        }
    }
}
