using System.Collections.Generic;
using _Game.Core._SceneLoader;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Services.Camera;
using Cysharp.Threading.Tasks;


namespace _Game.Core.GameState
{
    public class GameLoadingState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;

        public GameLoadingState(
            IGameStateMachine stateMachine,
            SceneLoader sceneLoader,
            IWorldCameraService cameraService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new GameLoadingOperation(
                    _sceneLoader, 
                    _cameraService));

            _stateMachine.Enter<MenuState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {

        }
    }
}