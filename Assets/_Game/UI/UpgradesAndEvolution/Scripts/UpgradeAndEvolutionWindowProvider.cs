using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionWindowProvider : LocalAssetLoader, IUpgradeAndEvolutionWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IPersistentDataService _persistentData;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAlertPopupProvider _alertPopupProvider;
        
        private readonly IHeader _header;
        
        private readonly IUpgradesService _upgradesService;
        private readonly IEvolutionService _evolutionService;

        public UpgradeAndEvolutionWindowProvider(
            IWorldCameraService cameraService,
            IPersistentDataService persistentData,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider,
            
            IHeader header,
            
            IUpgradesService upgradesService,
            IEvolutionService evolutionService)
        {
            _cameraService = cameraService;
            _persistentData = persistentData;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;
            
            _header = header;

            _upgradesService = upgradesService;
            _evolutionService = evolutionService;
        }
        public async UniTask<Disposable<UpgradeAndEvolutionWindow>> Load()
        {
            var popup = await LoadDisposable<UpgradeAndEvolutionWindow>(AssetsConstants.UPGRADE_AND_EVOLUTION_WINDOW);
            
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _persistentData,
                _audioService,
                _communicator,
                _alertPopupProvider,
                
                _header,
                
                _upgradesService,
                _evolutionService);
            return popup;
        }
    }
}
