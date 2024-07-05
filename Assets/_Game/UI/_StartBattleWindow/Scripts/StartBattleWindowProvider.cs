using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Navigation.Battle;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Gameplay.BattleLauncher;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI._StartBattleWindow.Scripts
{
    public  class StartBattleWindowProvider : LocalAssetLoader, IStartBattleWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IBattleLaunchManager _battleLaunchManager;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _persistentData;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IBattleNavigator _battleNavigator;

        public StartBattleWindowProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IBattleLaunchManager battleLaunchManager,
            IMyLogger logger,
            IUserContainer persistentData,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _battleLaunchManager = battleLaunchManager;
            _logger = logger;
            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
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
                _logger,
                _persistentData,
                _settingsPopupProvider,
                _battleNavigator);
            return window;
        }
    }
}