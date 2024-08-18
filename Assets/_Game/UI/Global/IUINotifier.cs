using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;

namespace _Game.UI.Global
{
    public interface IUINotifier
    {
        void Register(IUIListener listener);
        void Unregister(IUIListener listener);
        void OnScreenOpened(GameScreen screen);
        void OnScreenClosed(GameScreen screen);
    }
}