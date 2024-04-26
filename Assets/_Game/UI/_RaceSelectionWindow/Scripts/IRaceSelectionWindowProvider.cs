using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._RaceSelectionWindow.Scripts
{
    public interface IRaceSelectionWindowProvider
    {
        UniTask<Disposable<RaceSelectionWindow>> Load();
    }
}