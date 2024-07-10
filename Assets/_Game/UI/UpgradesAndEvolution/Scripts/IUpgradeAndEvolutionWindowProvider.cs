using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.UpgradesAndEvolution.Scripts
{
    public interface IUpgradeAndEvolutionWindowProvider
    {
        UniTask<Disposable<UpgradeAndEvolutionWindow>> Load();
    }
}