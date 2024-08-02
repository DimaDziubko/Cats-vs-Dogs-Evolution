using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay.BattleLauncher;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public  class StartBattleScreenProvider : LocalAssetLoader, IStartBattleScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IBattleLaunchManager _battleLaunchManager;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _persistentData;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineConfigRepository _timelineConfigRepository;

        public StartBattleScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IBattleLaunchManager battleLaunchManager,
            IMyLogger logger,
            IUserContainer persistentData,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            ITimelineConfigRepository timelineConfigRepository)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _battleLaunchManager = battleLaunchManager;
            _logger = logger;
            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _timelineConfigRepository = timelineConfigRepository;
        }
        public async UniTask<Disposable<StartBattleScreen>> Load()
        {
            var window = await
                LoadDisposable<StartBattleScreen>(AssetsConstants.START_BATTLE_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _battleLaunchManager,
                _logger,
                _persistentData,
                _settingsPopupProvider,
                _battleNavigator,
                _timelineNavigator,
                _ageNavigator,
                _timelineConfigRepository);
            return window;
        }
    }
}