using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public interface IUpgradeAndEvolutionScreenProvider
    {
        UniTask<Disposable<UpgradeAndEvolutionScreen>> Load();
    }
}