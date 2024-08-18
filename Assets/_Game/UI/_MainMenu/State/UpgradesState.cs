using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class UpgradesState : IMenuState
    {
        private readonly MainMenu _mainMenu;
        private readonly IUpgradeAndEvolutionScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;
        
        private Disposable<UpgradeAndEvolutionScreen> _upgradesAndEvolutionScreen;
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
            _upgradesAndEvolutionScreen = await _provider.Load();

            if (_upgradesAndEvolutionScreen?.Value != null)
            {
                _upgradesAndEvolutionScreen.Value.Show();
                _uiNotifier.OnScreenOpened(GameScreen.UpgradesAndEvolution);
            }
        }

        public void Exit()
        {
            if (_upgradesAndEvolutionScreen?.Value != null)
            {
                _upgradesAndEvolutionScreen.Value.Hide();
                _upgradesAndEvolutionScreen.Dispose();
                _upgradesAndEvolutionScreen = null;
            }
            _button.UnHighlightBtn();
            _uiNotifier.OnScreenClosed(GameScreen.UpgradesAndEvolution);
        }

        public void Cleanup()
        {
            if (_upgradesAndEvolutionScreen?.Value != null)
            {
                _upgradesAndEvolutionScreen.Value.Hide();
                _upgradesAndEvolutionScreen.Dispose();
                _upgradesAndEvolutionScreen = null;
            }
            _button.UnHighlightBtn();
        }
    }
}