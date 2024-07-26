using _Game.Core.AssetManagement;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._RaceSelectionWindow.Scripts
{
    public class RaceSelectionWindowProvider : LocalAssetLoader, IRaceSelectionWindowProvider
    {
        private readonly IUserContainer _persistentData;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;
        private readonly ITutorialManager _tutorialManager;

        public RaceSelectionWindowProvider(
            IUserContainer persistentData,
            IAudioService audioService,
            IWorldCameraService cameraService,
            ITutorialManager tutorialManager)
        {
            _persistentData = persistentData;
            _audioService = audioService;
            _cameraService = cameraService;
            _tutorialManager = tutorialManager;
        }
        public async UniTask<Disposable<RaceSelectionWindow>> Load()
        {
            var window = await
                LoadDisposable<RaceSelectionWindow>(AssetsConstants.RACE_SELECTION_WINDOW);
            window.Value.Construct(
                _persistentData,
                _audioService,
                _cameraService,
                _tutorialManager);
            return window;
        }
        
    }
}
