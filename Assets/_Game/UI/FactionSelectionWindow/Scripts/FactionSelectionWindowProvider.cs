using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.FactionSelectionWindow.Scripts
{
    public class FactionSelectionWindowProvider : LocalAssetLoader, IFactionSelectionWindowProvider
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;

        public FactionSelectionWindowProvider(
            IPersistentDataService persistentData,
            IAudioService audioService,
            IWorldCameraService cameraService)
        {
            _persistentData = persistentData;
            _audioService = audioService;
            _cameraService = cameraService;
        }
        public async UniTask<Disposable<FactionSelectionWindow>> Load()
        {
            var window = await
                LoadDisposable<FactionSelectionWindow>(AssetsConstants.FACTION_SELECTION_WINDOW);
            window.Value.Construct(
                _persistentData,
                _audioService,
                _cameraService);
            return window;
        }
        
    }
}
