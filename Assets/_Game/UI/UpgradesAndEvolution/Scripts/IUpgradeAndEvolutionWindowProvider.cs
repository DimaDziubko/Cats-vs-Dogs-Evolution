using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public interface IUpgradeAndEvolutionWindowProvider
    {
        UniTask<Disposable<UpgradeAndEvolutionWindow>> Load();
    }
}