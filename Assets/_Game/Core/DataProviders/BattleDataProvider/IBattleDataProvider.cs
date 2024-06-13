using _Game.Core.Data;
using _Game.Core.Data.Battle;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.BattleDataProvider
{
    public interface IBattleDataProvider
    {
        UniTask<BattleStaticData> Load();
    }
}