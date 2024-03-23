using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay.GameResult.Scripts
{
    public interface IGameResultWindowProvider
    {
        UniTask<Disposable<GameResultWindow>> Load();
    }
}