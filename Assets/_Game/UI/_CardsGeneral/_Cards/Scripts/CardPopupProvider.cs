using _Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardPopupProvider
    {
        UniTask<Disposable<CardPopup>> Load();
    }

    public class CardPopupProvider : LocalAssetLoader, ICardPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly ICardsPresenter _cardPresenter;
        private readonly IAudioService _audioService;

        public CardPopupProvider(
            IWorldCameraService cameraService,
            ICardsPresenter cardsPresenter,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _cardPresenter = cardsPresenter;
            _audioService = audioService;
        }
        
        public async UniTask<Disposable<CardPopup>> Load()
        {
            var popup = await LoadDisposable<CardPopup>(AssetsConstants.CARD_POPUP);
            popup.Value.Construct(_cameraService, _cardPresenter, _audioService);
            return popup;
        }
    }
}