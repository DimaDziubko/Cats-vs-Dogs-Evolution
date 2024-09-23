using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardPopupProvider
    {
        UniTask<Disposable<CardPopup>> Load();
        void Unload();
    }

    public class CardPopupProvider : LocalAssetLoader, ICardPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly ICardsPresenter _cardPresenter;
        private readonly IAudioService _audioService;
        private readonly IBoostDataPresenter _boostDataPresenter;

        public CardPopupProvider(
            IWorldCameraService cameraService,
            ICardsPresenter cardsPresenter,
            IAudioService audioService,
            IBoostDataPresenter boostDataPresenter)
        {
            _cameraService = cameraService;
            _cardPresenter = cardsPresenter;
            _audioService = audioService;
            _boostDataPresenter = boostDataPresenter;
        }
        
        private Disposable<CardPopup> _popup;
        
        public async UniTask<Disposable<CardPopup>> Load()
        {
            if (_popup != null) return _popup;
            _popup = await LoadDisposable<CardPopup>(AssetsConstants.CARD_POPUP);
            _popup.Value.Construct(_cameraService, _cardPresenter, _boostDataPresenter, _audioService);
            return _popup;
        }
        
        public override void Unload()
        {
            if (_popup != null)
            {
                _popup.Value.Cleanup();
                _popup.Dispose();
                _popup = null;
            }
            
            base.Unload();
        }
    }
}