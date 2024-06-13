using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Navigation;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils;

namespace _Game.Core.DataPresenters.WeaponDataPresenter
{
    public class WeaponDataPresenter : IWeaponDataPresenter
    {
        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;

        public WeaponDataPresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger
        )
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
        }
        
        public WeaponData GetWeaponData(WeaponType type, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                return _dataPool.AgeStaticData.ForWeapon(type);
            }
            else if(context == Constants.CacheContext.BATTLE)
            {
                return _dataPool.BattleStaticData.ForWeapon(_navigator.CurrentBattle, type);
            }
            else
            {
                _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
                return null;
            }
        }
    }
}