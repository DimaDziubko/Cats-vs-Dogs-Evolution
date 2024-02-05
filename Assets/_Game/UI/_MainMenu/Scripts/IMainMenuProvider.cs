using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.Scripts
{
    public interface IMainMenuProvider
    {
        UniTask<Disposable<MainMenu>> Load();
    }
}