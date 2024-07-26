using System;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataPresenters._BaseDataPresenter
{
    public interface IBasePresenter
    {
        event Action<Faction> BaseUpdated;
        BaseModel GetBaseModel(int context);
        float GetBaseHealth(Faction faction);
    }
}