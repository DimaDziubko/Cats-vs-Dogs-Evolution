using System;
using Assets._Game.Gameplay.Battle.Scripts;

namespace Assets._Game.Core.DataPresenters.BattlePresenter
{
    public interface IBattlePresenter
    {
        BattleData BattleData { get;}

        event Action<BattleData> BattleDataUpdated;
    }
}