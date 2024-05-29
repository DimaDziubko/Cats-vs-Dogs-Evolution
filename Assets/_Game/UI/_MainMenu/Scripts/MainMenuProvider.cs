using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Pin.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.Scripts
{
    public class MainMenuProvider : LocalAssetLoader, IMainMenuProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IStartBattleWindowProvider _startBattleWindowProvide;
        private readonly IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly ITutorialManager _tutorialManager;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IMyLogger _logger;

        public MainMenuProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IStartBattleWindowProvider startBattleWindowProvider,
            IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _startBattleWindowProvide = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
            _logger = logger;
        }
        public async UniTask<Disposable<MainMenu>> Load()
        {
            var popup = await LoadDisposable<MainMenu>(AssetsConstants.MAIN_MENU);
            
            popup.Value.Construct(
                _cameraService,
                _audioService,
                _startBattleWindowProvide,
                _upgradeAndEvolutionWindowProvider,
                _featureUnlockSystem,
                _tutorialManager,
                _upgradesChecker,
                _logger);
            return popup;
        }
    }
}