using System;
using _Game.Gameplay._Boosts.Scripts;

namespace _Game.Core.Data.Age.Dynamic._UpgradeItem
{
    public interface IBoostsDataReadonly
    {
        event Action<BoostSource, BoostType, float> Changed;
        float GetBoost(BoostSource source, BoostType type);
    }
}