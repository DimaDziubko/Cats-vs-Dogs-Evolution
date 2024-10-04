using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class UpgradesState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IUpgradeAndEvolutionScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;

        private readonly ToggleButton _button;

        public UpgradesState(
            MainMenu mainMenu,
            IUpgradeAndEvolutionScreenProvider provider,
            ToggleButton button,
            IUINotifier uiNotifier)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _button = button;
            _uiNotifier = uiNotifier;
        }
        
        public async void Enter()
        {
            _mainMenu.SetActiveButton(_button);
            _button.HighlightBtn();
            _mainMenu.RebuildLayout();
            
            var upgradesAndEvolutionScreen = await _provider.Load();
            upgradesAndEvolutionScreen.Value.Show();
            _uiNotifier.OnScreenOpened(GameScreen.UpgradesAndEvolution);
            
            _mainMenu.UpgradeTutorialStep.CancelStep();
        }

        public void Exit()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.UpgradesAndEvolution);
            
            _mainMenu.RebuildLayout();
            _mainMenu.ShowUpgradeTutorialWithDelay(0.5f);
        }

        public void Cleanup()
        {
            _provider.Unload();
            _button.UnHighlightBtn();
        }
    }
}