using System.Collections.Generic;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using Assets._Game.Core._SceneLoader;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;
using Assets._Game.Core.Services.Camera;

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
            
            var data = new LoadingData()
            {
                Type = LoadingScreenType.Simple,
                Operations = loadingOperations,
            };
            
            _stateMachine.Enter<MenuState, LoadingData>(data);
        }

        public void Exit()
        {

        }
    }
}