using _Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardAppearancePopupProvider : LocalAssetLoader, ICardAppearancePopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private CardsSummoningModel _cardsSummoningModel;

        public CardAppearancePopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }

        public async UniTask<Disposable<CardAppearancePopup>> Load()
        {
            var popup = await LoadDisposable<CardAppearancePopup>(AssetsConstants.CARD_APPEARANCE_POPUP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService);
            return popup;
        }
    }
}