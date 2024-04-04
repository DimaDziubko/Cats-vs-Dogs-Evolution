using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionWindowProvider : LocalAssetLoader, IUpgradeAndEvolutionWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IAlertPopupProvider _alertPopupProvider;
        
        private readonly IHeader _header;
        
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IEvolutionService _evolutionService;
        private readonly IUnitUpgradesService _unitUpgradesService;
        private readonly IAgeStateService _ageState;
        private readonly IMyLogger _logger;
        private readonly ITimelineInfoWindowProvider _timelineInfoWindowProvider;

        public UpgradeAndEvolutionWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IAlertPopupProvider alertPopupProvider,
            IHeader header,
            IEconomyUpgradesService economyUpgradesService,
            IEvolutionService evolutionService,
            IUnitUpgradesService unitUpgradesService,
            IAgeStateService ageState,
            IMyLogger logger,
            ITimelineInfoWindowProvider timelineInfoWindowProvider)
        {
            _logger = logger;
            
            _cameraService = cameraService;
            _audioService = audioService;
            _alertPopupProvider = alertPopupProvider;
            
            _header = header;

            _economyUpgradesService = economyUpgradesService;
            _evolutionService = evolutionService;
            _unitUpgradesService = unitUpgradesService;

            _ageState = ageState;

            _timelineInfoWindowProvider = timelineInfoWindowProvider;
        }
        public async UniTask<Disposable<UpgradeAndEvolutionWindow>> Load()
        {
            var popup = await LoadDisposable<UpgradeAndEvolutionWindow>(AssetsConstants.UPGRADE_AND_EVOLUTION_WINDOW);
            
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _alertPopupProvider,
                
                _header,
                
                _economyUpgradesService,
                _evolutionService,
                _unitUpgradesService,
                _logger,
                
                _timelineInfoWindowProvider);
            return popup;
        }
    }
}
