using Assets._Game.Core.Loading;
using Assets._Game.Core.LoadingScreen;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.UI._MainMenu.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.GameState
{
    public class MenuState : IPayloadedState<LoadingData>
    {
        private const string ANALYTICS_EVENT_NAME = "main_menu";
        
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly IGameStateMachine _stateMachine;
        private readonly IDTDAnalyticsService _dtdAnalyticsService;
        private readonly IAnalyticsService _analyticsService;

        private Disposable<MainMenu> _mainMenu;

        public MenuState(
            IMainMenuProvider mainMenuProvider,
            ILoadingScreenProvider loadingProvider,
            IGameStateMachine stateMachine,
            IDTDAnalyticsService dtdAnalyticsService,
            IAnalyticsService analyticsService)
        {
            _mainMenuProvider = mainMenuProvider;
            _loadingProvider = loadingProvider;
            _stateMachine = stateMachine;
            _dtdAnalyticsService = dtdAnalyticsService;
            _analyticsService = analyticsService;
        }
        
        public void Enter(LoadingData data)
        {
            data.Operations.Enqueue(new MainMenuLoadingOperation(_mainMenuProvider));
            _loadingProvider.LoadAndDestroy(data.Operations, data.Type).Forget();
            
            _dtdAnalyticsService.SendEvent(ANALYTICS_EVENT_NAME);
            _analyticsService.SendEvent(ANALYTICS_EVENT_NAME);
        }
        
        public void Exit()
        {
            _mainMenuProvider.HideMainMenu();
            _mainMenuProvider.Unload();
        }
    }
}
