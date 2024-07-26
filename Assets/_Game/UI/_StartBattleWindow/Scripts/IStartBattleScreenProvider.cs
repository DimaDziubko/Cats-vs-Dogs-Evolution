using _Game.UI._StartBattleWindow.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI._StartBattleWindow.Scripts
{
    public interface IStartBattleScreenProvider
    {
        UniTask<Disposable<StartBattleScreen>> Load();
    }
}