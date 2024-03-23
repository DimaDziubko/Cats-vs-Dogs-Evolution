using System.Collections.Generic;
using _Game.Core.Loading;
using _Game.Core.Scripts;
using _Game.Core.Services.Camera;
using Cysharp.Threading.Tasks;


namespace _Game.Core.GameState
{
    public class GameLoadingState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly ILoadingScreenProvider _loadingProvider;

        public GameLoadingState(
            IGameStateMachine stateMachine,
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            ILoadingScreenProvider loadingProvider)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _loadingProvider = loadingProvider;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new GameLoadingOperation(
                    _sceneLoader, 
                    _cameraService, 
                    _stateMachine));

            _loadingProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingProvider.LoadAndDestroy(loadingOperations).Forget();
            
        }

        public void Exit()
        {

        }

        private void OnLoadingCompleted()
        {
            _loadingProvider.LoadingCompleted -= OnLoadingCompleted;
            _stateMachine.Enter<MenuState>();
        }
    }
}