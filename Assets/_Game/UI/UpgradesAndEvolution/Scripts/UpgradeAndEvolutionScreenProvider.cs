using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.AssetManagement;
using _Game.Core.DataPresenters._UpgradeItemPresenter;
using _Game.Core.DataPresenters.Evolution;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Header.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionScreenProvider : LocalAssetLoader, IUpgradeAndEvolutionScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private readonly IHeader _header;
        
        private readonly IUpgradeItemPresenter _upgradeItemPresenter;
        private readonly IEvolutionPresenter _evolutionPresenter;
        private readonly IUnitUpgradesPresenter _unitUpgradesPresenter;
        private readonly IMyLogger _logger;
        private readonly ITimelineInfoScreenProvider _timelineInfoScreenProvider;
        private readonly ITutorialManager _tutorialManager;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly ITimelineTravelPresenter _timelineTravelPresenter;
        private readonly IMiniShopProvider _miniShopProvider;

        public UpgradeAndEvolutionScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IUpgradeItemPresenter upgradeItemPresenter,
            IEvolutionPresenter evolutionPresenter,
            IUnitUpgradesPresenter unitUpgradesPresenter,
            IMyLogger logger,
            ITimelineInfoScreenProvider timelineInfoScreenProvider,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IUpgradesAvailabilityChecker upgradesChecker, 
            ITimelineTravelPresenter timelineTravelPresenter,
            IMiniShopProvider miniShopProvider)
        {
            _logger = logger;
            
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _upgradeItemPresenter = upgradeItemPresenter;
            _evolutionPresenter = evolutionPresenter;
            _unitUpgradesPresenter = unitUpgradesPresenter;
            _timelineInfoScreenProvider = timelineInfoScreenProvider;
            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;
            _upgradesChecker = upgradesChecker;
            _timelineTravelPresenter = timelineTravelPresenter;
            _miniShopProvider = miniShopProvider;
        }
        public async UniTask<Disposable<UpgradeAndEvolutionScreen>> Load()
        {
            var popup = await LoadDisposable<UpgradeAndEvolutionScreen>(AssetsConstants.UPGRADE_AND_EVOLUTION_WINDOW);
            
            popup.Value.Construct(
                _cameraService,
                _audioService,
                _header,
                _upgradeItemPresenter,
                _evolutionPresenter,
                _timelineTravelPresenter,
                _unitUpgradesPresenter,
                _logger,
                _timelineInfoScreenProvider,
                _tutorialManager,
                _featureUnlockSystem,
                _upgradesChecker, 
                _miniShopProvider);
            return popup;
        }
    }
}
