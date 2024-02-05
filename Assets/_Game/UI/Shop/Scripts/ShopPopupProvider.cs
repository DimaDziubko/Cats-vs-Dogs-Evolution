using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Shop.Scripts
{
    public class ShopPopupProvider : LocalAssetLoader, IShopPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IPersistentDataService _persistentData;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAlertPopupProvider _alertPopupProvider;

        public ShopPopupProvider(
            IWorldCameraService cameraService,
            IPersistentDataService persistentData,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider)
        {
            _cameraService = cameraService;
            _persistentData = persistentData;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;
        }
        public async UniTask<Disposable<ShopPopup>> Load()
        {
            var popup = await LoadDisposable<ShopPopup>(AssetsConstants.SHOP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _persistentData,
                _audioService,
                _communicator,
                _alertPopupProvider);
            return popup;
        }
    }
}