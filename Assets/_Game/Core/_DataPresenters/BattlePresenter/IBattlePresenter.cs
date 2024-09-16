using System;
using _Game.Gameplay._Battle.Scripts;

namespace _Game.Core.DataPresenters.BattlePresenter
{
    public interface IBattlePresenter
    {
        BattleData BattleData { get;}

        event Action<BattleData, bool> BattleDataUpdated;
    }
}