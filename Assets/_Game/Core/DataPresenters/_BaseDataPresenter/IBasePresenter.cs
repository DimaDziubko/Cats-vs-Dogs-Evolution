using System;
using _Game.Gameplay._Bases.Scripts;

namespace _Game.Core.DataPresenters._BaseDataPresenter
{
    public interface IBasePresenter
    {
        event Action PlayerBaseUpdated;
        event Action<BaseModel> PlayerBaseDataUpdated;
        BaseModel GetTowerData(int context);
    }
}