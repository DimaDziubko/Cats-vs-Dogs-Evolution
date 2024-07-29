using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;

namespace _Game.UI._MainMenu.State
{
    public class BattleState : IMenuState
    {
        private readonly MainMenu _mainMenu;
        private readonly IStartBattleScreenProvider _provider;

        private Disposable<StartBattleScreen> _startBattleWindow;
        private readonly ToggleButton _button;

        public BattleState(
            MainMenu mainMenu,
            IStartBattleScreenProvider provider,
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
            _startBattleWindow = await _provider.Load();
            _startBattleWindow.Value.Show();
        }

        public void Exit()
        {
            _startBattleWindow.Value.Hide();
            _startBattleWindow.Dispose();
            _startBattleWindow = null;
            _button.UnHighlightBtn();
        }

        public void Cleanup()
        {
            if (_startBattleWindow != null)
            {
                _startBattleWindow.Value.Hide();
                _startBattleWindow.Dispose();
                _startBattleWindow = null;
                _button.UnHighlightBtn();
            }
        }
    }
}