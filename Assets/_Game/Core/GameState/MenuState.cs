using System.Collections.Generic;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.Core.GameState
{
    public class MenuState : IState, IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly IGameStateMachine _stateMachine;

        private Disposable<MainMenu> _mainMenu;

        public MenuState(
            IMainMenuProvider mainMenuProvider,
            ILoadingScreenProvider loadingProvider,
            IGameStateMachine stateMachine)
        {
            _mainMenuProvider = mainMenuProvider;
            _loadingProvider = loadingProvider;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _loadingProvider.LoadAndDestroy(new MainMenuLoadingOperation(_mainMenuProvider), LoadingScreenType.DarkFade);
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new MainMenuLoadingOperation(_mainMenuProvider));
            _loadingProvider.LoadAndDestroy(loadingOperations, LoadingScreenType.Simple).Forget();
        }
        
        public void Exit()
        {
            _mainMenuProvider.HideMainMenu();
            _mainMenuProvider.Unload();
        }
    }
}
