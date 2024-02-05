using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public interface IStartBattleWindowProvider
    {
        UniTask<Disposable<StartBattleWindow>> Load();
    }
}