using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI._MainMenu.Scripts
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
        
        private Disposable<MainMenu> _mainMenu;

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
        public async UniTask Load()
        {
            _mainMenu = await LoadDisposable<MainMenu>(AssetsConstants.MAIN_MENU);
            
            _mainMenu.Value.Construct(
                _cameraService,
                _audioService,
                _startBattleWindowProvide,
                _upgradeAndEvolutionWindowProvider,
                _featureUnlockSystem,
                _tutorialManager,
                _upgradesChecker,
                _logger);

            ShowMainMenu();
        }
        
        public void Unload()
        {
            if (_mainMenu != null)
            {
                _mainMenu.Dispose();
                _mainMenu = null;
            }
        }

        private void ShowMainMenu()
        {
            if (_mainMenu != null)
            {
                _mainMenu.Value.Show();
            }
        }

        public void HideMainMenu()
        {
            if (_mainMenu != null)
            {
                _mainMenu.Value.Hide();
            }
        }
    }
}