using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.AssetManagement;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI.Global;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreenProvider : LocalAssetLoader, IGeneralCardsScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly ICardsScreenProvider _cardsScreenProvider;
        private readonly IUINotifier _uiNotifier;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IAudioService _audioService;

        public GeneralCardsScreenProvider(
            IWorldCameraService cameraService,
            ICardsScreenProvider cardsScreenProvider,
            IUINotifier uiNotifier,
            IFeatureUnlockSystem featureUnlockSystem,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _cardsScreenProvider = cardsScreenProvider;
            _uiNotifier = uiNotifier;
            _featureUnlockSystem = featureUnlockSystem;
            _audioService = audioService;
        }

            public async UniTask<Disposable<GeneralCardsScreen>> Load()
        {
            var popup = await LoadDisposable<GeneralCardsScreen>(AssetsConstants.GENERAL_CARDS_SCREEN);
            popup.Value.Construct(
                _cameraService, 
                _cardsScreenProvider, 
                _uiNotifier,
                _featureUnlockSystem,
                _audioService);
            return popup;
        }
    }
}