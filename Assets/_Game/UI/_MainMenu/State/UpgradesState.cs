using _Game.UI._MainMenu.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class UpgradesState : IMenuState
    {
        private readonly MainMenu _mainMenu;
        private readonly IUpgradeAndEvolutionScreenProvider _provider;

        private Disposable<UpgradeAndEvolutionScreen> _upgradesAndEvolutionScreen;
        private readonly ToggleButton _button;

        public UpgradesState(
            MainMenu mainMenu,
            IUpgradeAndEvolutionScreenProvider provider,
            ToggleButton button)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _button = button;
        }
        
        public async void Enter()
        {
            _mainMenu.SetActiveButton(_button);
            _button.HighlightBtn();
            _mainMenu.RebuildLayout();
            _upgradesAndEvolutionScreen = await _provider.Load();
            _upgradesAndEvolutionScreen.Value.Show();
        }

        public void Exit()
        {
            _upgradesAndEvolutionScreen.Value.Hide();
            _upgradesAndEvolutionScreen.Dispose();
            _upgradesAndEvolutionScreen = null;
            _button.UnHighlightBtn();
        }

        public void Cleanup()
        {
            if (_upgradesAndEvolutionScreen != null)
            {
                _upgradesAndEvolutionScreen.Value.Hide();
                _upgradesAndEvolutionScreen.Dispose();
                _upgradesAndEvolutionScreen = null;
                _button.UnHighlightBtn();
            }
        }
    }
}