using Assets._Game.Core.Data.Battle;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.BattleDataProvider
{
    public interface IBattleDataProvider
    {
        UniTask<BattleStaticData> Load();
    }
}