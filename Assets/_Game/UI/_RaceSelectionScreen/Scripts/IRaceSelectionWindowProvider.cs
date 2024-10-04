using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._RaceSelectionScreen.Scripts
{
    public interface IRaceSelectionWindowProvider
    {
        UniTask<Disposable<RaceSelectionWindow>> Load();
    }
}