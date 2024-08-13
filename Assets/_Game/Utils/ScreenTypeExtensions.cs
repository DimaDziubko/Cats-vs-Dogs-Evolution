using Screen = _Game.UI._MainMenu.Scripts.Screen;

namespace _Game.Utils
{
    public static class ScreenTypeExtensions
    {
        public static bool IsComposite(this Screen screen)
        {
            switch (screen)
            {
                case Screen.UpgradesAndEvolution:
                    return true;
                case Screen.Upgrades:
                    return true;
                default:
                    return false;
            }
        }
    }

}