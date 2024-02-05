using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public  class StartBattleWindowProvider : LocalAssetLoader, IStartBattleWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IPersistentDataService _persistentData;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAlertPopupProvider _alertPopupProvider;
        
        private readonly IHeader _header;
        
        private readonly IBeginGameManager _beginGameManager;
        
        private readonly IGameConfigController _gameConfigController;
        private readonly IBattleStateService _battleState;

        public StartBattleWindowProvider(
            IWorldCameraService cameraService,
            IPersistentDataService persistentData,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider,
            
            IHeader header,
            IBeginGameManager beginGameManager,
            
            IGameConfigController gameConfigController,

            IBattleStateService battleState)
        {
            _cameraService = cameraService;
            _persistentData = persistentData;
            _audioService = audioService;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;

            _header = header;

            _beginGameManager = beginGameManager;

            _gameConfigController = gameConfigController;

            _battleState = battleState;
        }
        public async UniTask<Disposable<StartBattleWindow>> Load()
        {
            var window = await
                LoadDisposable<StartBattleWindow>(AssetsConstants.START_BATTLE_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _persistentData,
                _audioService,
                _communicator,
                
                _header,
                
                _beginGameManager,
                
                _gameConfigController,

                _battleState);
            return window;
        }
    }
}