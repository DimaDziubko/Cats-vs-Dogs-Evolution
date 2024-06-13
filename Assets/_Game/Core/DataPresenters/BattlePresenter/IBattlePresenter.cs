using System;
using _Game.Gameplay.Battle.Scripts;

namespace _Game.Core.DataPresenters.BattlePresenter
{
    public interface IBattlePresenter
    {
        BattleData BattleData { get;}

        event Action<BattleData> BattleDataUpdated;
    }
}