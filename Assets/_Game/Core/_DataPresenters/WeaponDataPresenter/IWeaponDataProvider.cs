using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core._DataPresenters.WeaponDataPresenter
{
    public interface IWeaponDataProvider
    {
        WeaponData GetWeaponData(int weaponId, int context);
    }
}