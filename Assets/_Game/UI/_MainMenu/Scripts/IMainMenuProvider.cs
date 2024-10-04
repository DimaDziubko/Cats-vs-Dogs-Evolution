using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.Scripts
{
    public interface IMainMenuProvider
    {
        UniTask Load();
        void Unload();
        void HideMainMenu();
    }
}