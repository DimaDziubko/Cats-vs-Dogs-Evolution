using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.Scripts
{
    public class MainMenuProvider : LocalAssetLoader, IMainMenuProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IPersistentDataService _persistentData;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAlertPopupProvider _alertPopupProvider;
        
        private readonly IStartBattleWindowProvider _startBattleWindowProvide;
        private readonly IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;

        public MainMenuProvider(
            IWorldCameraService cameraService,
            IPersistentDataService persistentData,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider,
            
            IStartBattleWindowProvider startBattleWindowProvider,
            IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider)
        {
            _cameraService = cameraService;
            _persistentData = persistentData;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;
            _startBattleWindowProvide = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;
        }
        public async UniTask<Disposable<MainMenu>> Load()
        {
            var popup = await LoadDisposable<MainMenu>(AssetsConstants.MAIN_MENU);
            
            popup.Value.Construct(
                _cameraService,
                _persistentData,
                _audioService,
                _communicator,
                _startBattleWindowProvide,
                _upgradeAndEvolutionWindowProvider);
            return popup;
        }
    }
}