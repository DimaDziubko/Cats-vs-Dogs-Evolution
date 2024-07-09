using _Game.Core.DataPresenters.WeaponDataPresenter;
using _Game.Core.Navigation.Battle;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Utils;

namespace Assets._Game.Core.DataPresenters.WeaponDataPresenter
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
        
        public WeaponData GetWeaponData(int weaponId, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                return _dataPool.AgeStaticData.ForWeapon(weaponId);
            }
            else if(context == Constants.CacheContext.BATTLE)
            {
                return _dataPool.BattleStaticData.ForWeapon(_navigator.CurrentBattle, weaponId);
            }
            else
            {
                _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
                return null;
            }
        }
    }
}