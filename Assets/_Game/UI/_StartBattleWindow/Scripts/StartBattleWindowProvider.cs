using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay.BattleLauncher;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public  class StartBattleWindowProvider : LocalAssetLoader, IStartBattleWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IBattleLaunchManager _battleLaunchManager;
        private readonly IBattleStateService _battleState;
        private readonly IMyLogger _logger;
        private readonly IPersistentDataService _persistentData;
        private readonly ISettingsPopupProvider _settingsPopupProvider;

        public StartBattleWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IBattleLaunchManager battleLaunchManager,
            IBattleStateService battleState,
            IMyLogger logger,
            IPersistentDataService persistentData,
            ISettingsPopupProvider settingsPopupProvider)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _battleLaunchManager = battleLaunchManager;
            _battleState = battleState;
            _logger = logger;
            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
        }
        public async UniTask<Disposable<StartBattleWindow>> Load()
        {
            var window = await
                LoadDisposable<StartBattleWindow>(AssetsConstants.START_BATTLE_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _battleLaunchManager,
                _battleState,
                _logger,
                _persistentData,
                _settingsPopupProvider);
            return window;
        }
    }
}