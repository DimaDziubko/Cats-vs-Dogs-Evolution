using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;

        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;
        
        public UnitDataProvider(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger
            )
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
        }
        
        public IUnitData GetDecoratedUnitData(UnitType type, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                IUnitData vanillaData =  _dataPool.AgeStaticData.ForUnit(type);
                DamageBoostDecorator damageDecoratedData = new DamageBoostDecorator(vanillaData, 
                    BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.AllUnitDamage));
                HealthBoostDecorator healthBoostDecorator = new HealthBoostDecorator(damageDecoratedData, 
                    BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.AllUnitHealth));
                return healthBoostDecorator;
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