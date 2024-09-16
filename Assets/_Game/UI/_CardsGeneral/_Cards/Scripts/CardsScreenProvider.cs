using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenProvider : LocalAssetLoader, ICardsScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly ICardsScreenPresenter _cardsScreenPresenter;
        private readonly IMyLogger _logger;
        private readonly IUIFactory _uiFactory;
        private readonly IHeader _header;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly ICardsPresenter _cardsPresenter;
        private readonly ITutorialManager _tutorialManager;

        public CardsScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            ICardsScreenPresenter cardsScreenPresenter,
            IUIFactory uiFactory,
            IMyLogger logger,
            IHeader header,
            IUpgradesAvailabilityChecker upgradesChecker,
            ICardsPresenter cardsPresenter,
            ITutorialManager tutorialManager)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsScreenPresenter = cardsScreenPresenter;
            _uiFactory = uiFactory;
            _logger = logger;
            _header = header;
            _upgradesChecker = upgradesChecker;
            _cardsPresenter = cardsPresenter;
            _tutorialManager = tutorialManager;
        }
        
        public async UniTask<Disposable<CardsScreen>> Load()
        {
            var popup = await LoadDisposable<CardsScreen>(AssetsConstants.CARDS_SCREEN);
            popup.Value.Construct(
                _cameraService, 
                _audioService,
                _cardsScreenPresenter, 
                _uiFactory,
                _logger,
                _header,
                _upgradesChecker,
                _cardsPresenter,
                _tutorialManager);
            return popup;
        }
    }
}