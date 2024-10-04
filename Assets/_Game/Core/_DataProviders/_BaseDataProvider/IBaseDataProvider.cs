using System;
using _Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders._BaseDataProvider
{
    public interface IBaseDataProvider
    {
        event Action<Faction> BaseUpdated;
        IBaseData GetBaseData(int context);
    }
}