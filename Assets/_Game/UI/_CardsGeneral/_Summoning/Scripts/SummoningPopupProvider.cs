using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public interface ISummoningPopupProvider
    {
        UniTask<Disposable<SummoningPopup>> Load();
    }

    public class SummoningPopupProvider : LocalAssetLoader, ISummoningPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly CardsSummoningModel _cardsSummoningModel;

        public SummoningPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            CardsSummoningModel cardsSummoningModel)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsSummoningModel = cardsSummoningModel;
        }
        public async UniTask<Disposable<SummoningPopup>> Load()
        {
            var popup = await LoadDisposable<SummoningPopup>(AssetsConstants.SUMMONING_POPUP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _cardsSummoningModel);
            return popup;
        }
    }
}
