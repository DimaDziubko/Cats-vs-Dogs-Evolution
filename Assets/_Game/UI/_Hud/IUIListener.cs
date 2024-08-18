using _Game.UI._MainMenu.Scripts;

namespace _Game.UI._Hud
{
    public interface IUIListener
    {
        void OnScreenOpened(GameScreen gameScreen);
        void OnScreenClosed(GameScreen gameScreen);
    }
}