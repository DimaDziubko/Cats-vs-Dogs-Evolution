using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.GameResult.Scripts
{
    public interface IGameResultPopupProvider
    {
        UniTask<Disposable<GameResultPopup>> Load();
    }
}