using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Core.DataPresenters.WeaponDataPresenter
{
    public class WeaponDataPresenter : IWeaponDataPresenter
    {
        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;
        private readonly IAssetRegistry _assetRegistry;

        public WeaponDataPresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger,
            IAssetRegistry assetRegistry
        )
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
            _assetRegistry = assetRegistry;
        }
        
        public WeaponData GetWeaponData(int weaponId, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                return _dataPool.AgeStaticData.ForWeapon(weaponId);
            }

            if(context == Constants.CacheContext.BATTLE)
            {
                return _dataPool.BattleStaticData.ForWeapon(_navigator.CurrentBattle, weaponId);
            }

            _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
            return null;
        }
    }
}