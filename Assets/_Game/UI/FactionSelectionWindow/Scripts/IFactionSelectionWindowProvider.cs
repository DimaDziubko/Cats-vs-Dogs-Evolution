using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.FactionSelectionWindow.Scripts
{
    public interface IFactionSelectionWindowProvider
    {
        UniTask<Disposable<FactionSelectionWindow>> Load();
    }
}