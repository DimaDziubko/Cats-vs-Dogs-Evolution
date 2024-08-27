using _Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenProvider : LocalAssetLoader, ICardsScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly ICardsScreenPresenter _cardsScreenPresenter;

        public CardsScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            ICardsScreenPresenter cardsScreenPresenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsScreenPresenter = cardsScreenPresenter;
        }
        
        public async UniTask<Disposable<CardsScreen>> Load()
        {
            var popup = await LoadDisposable<CardsScreen>(AssetsConstants.CARDS_SCREEN);
            popup.Value.Construct(
                _cameraService, 
                _audioService,
                _cardsScreenPresenter
            );
            return popup;
        }
    }
}