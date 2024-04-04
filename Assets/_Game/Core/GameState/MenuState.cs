using System;
using _Game.Core.Loading;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;

namespace _Game.Core.GameState
{
    public class MenuState : IState
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IPersistentDataService _persistentDataService;
        private readonly IAgeStateService _ageState;
        private readonly IBattleStateService _battleState;

        private IRaceStateReadonly RaceStateReadonly => _persistentDataService.State.RaceState;

        private Disposable<MainMenu> _mainMenu;

        public MenuState(
            IMainMenuProvider mainMenuProvider,
            ILoadingScreenProvider loadingScreenProvider,
            IPersistentDataService persistentDataService,
            IAgeStateService ageState,
            IBattleStateService battleState)
        {
            _mainMenuProvider = mainMenuProvider;
            _loadingScreenProvider = loadingScreenProvider;
            _persistentDataService = persistentDataService;
            _ageState = ageState;
            _battleState = battleState;
        }

        public async void Enter()
        {
            _mainMenu = await _mainMenuProvider.Load();
            _mainMenu.Value.Show();

            RaceStateReadonly.Changed += OnFactionChanged;
        }

        public void Exit()
        {
            _mainMenu.Value.Hide();
            _mainMenu.Dispose();
            
            RaceStateReadonly.Changed -= OnFactionChanged;
        }

        private void OnFactionChanged()
        {
           var changeFactionOperation = new ChangingRaceOperation(_ageState, _battleState);
           _loadingScreenProvider.LoadAndDestroy(changeFactionOperation);
        }
    }
}
