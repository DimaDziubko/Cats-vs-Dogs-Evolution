using System;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Core.DataPresenters._BaseDataPresenter
{
    public interface IBasePresenter
    {
        event Action<Faction> BaseUpdated;
        event Action<BaseModel> PlayerBaseDataUpdated;
        BaseModel GetTowerData(int context);
    }
}