using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public  class StartBattleWindowProvider : LocalAssetLoader, IStartBattleWindowProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private readonly IHeader _header;
        
        private readonly IBeginGameManager _beginGameManager;
        
        private readonly IBattleStateService _battleState;
        
        private readonly IMyLogger _logger;

        public StartBattleWindowProvider(
            IWorldCameraService cameraService,
            
            IAudioService audioService,

            IHeader header,
            IBeginGameManager beginGameManager,

            IBattleStateService battleState,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _audioService = audioService;

            _header = header;

            _beginGameManager = beginGameManager;

            _battleState = battleState;

            _logger = logger;
        }
        public async UniTask<Disposable<StartBattleWindow>> Load()
        {
            var window = await
                LoadDisposable<StartBattleWindow>(AssetsConstants.START_BATTLE_WINDOW);
            
            window.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,

                _header,
                _beginGameManager,
                
                _battleState,
                _logger);
            return window;
        }
    }
}