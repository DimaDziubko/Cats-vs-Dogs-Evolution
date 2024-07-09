using Assets._Game.Gameplay._Weapon.Scripts;

namespace _Game.Core.DataPresenters.WeaponDataPresenter
{
    public interface IWeaponDataPresenter
    {
        WeaponData GetWeaponData(int weaponId, int context);
    }
}