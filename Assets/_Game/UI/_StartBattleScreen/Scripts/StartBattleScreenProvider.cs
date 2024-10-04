using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay.BattleLauncher;
using _Game.UI.Header.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleScreen.Scripts
{
    public  class StartBattleScreenProvider : LocalAssetLoader, IStartBattleScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IHeader _header;
        private readonly IBattleManager _battleManager;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _persistentData;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineConfigRepository _timelineConfigRepository;

        private Disposable<StartBattleScreen> _screen;
        
        public StartBattleScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IBattleManager battleManager,
            IMyLogger logger,
            IUserContainer persistentData,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            IConfigRepositoryFacade configRepositoryFacade)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _header = header;
            _battleManager = battleManager;
            _logger = logger;
            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
        }
        public async UniTask<Disposable<StartBattleScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await
                LoadDisposable<StartBattleScreen>(AssetsConstants.START_BATTLE_WINDOW);
            
            _screen.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _header,
                _battleManager,
                _logger,
                _persistentData,
                _settingsPopupProvider,
                _battleNavigator,
                _timelineNavigator,
                _ageNavigator,
                _timelineConfigRepository);
            return _screen;
        }
        
        public override void Unload()
        {
            if (_screen != null)
            {
                _screen.Value.Hide();
                _screen.Dispose();
                _screen = null;
            }

            base.Unload();
        }
    }
}