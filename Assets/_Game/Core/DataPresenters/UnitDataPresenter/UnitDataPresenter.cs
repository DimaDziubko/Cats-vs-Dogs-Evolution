using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;

namespace _Game.Core.DataPresenters.UnitDataPresenter
{
    public class UnitDataPresenter : IUnitDataPresenter
    {
        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;

        public UnitDataPresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger
            )
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
        }
        
        public UnitData GetUnitData(UnitType type, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                return _dataPool.AgeStaticData.ForUnit(type);
            }
            else if(context == Constants.CacheContext.BATTLE)
            {
                return _dataPool.BattleStaticData.ForUnit(_navigator.CurrentBattle, type);
            }
            else
            {
                _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
                return null;
            }
        }
    }
}