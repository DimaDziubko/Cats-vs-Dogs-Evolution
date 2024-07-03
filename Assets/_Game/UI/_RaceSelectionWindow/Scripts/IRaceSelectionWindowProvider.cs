using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI._RaceSelectionWindow.Scripts
{
    public interface IRaceSelectionWindowProvider
    {
        UniTask<Disposable<RaceSelectionWindow>> Load();
    }
}