using _Game.UI.GameResult.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Gameplay.GameResult.Scripts
{
    public interface IGameResultWindowProvider
    {
        UniTask<Disposable<GameResultWindow>> Load();
    }
}