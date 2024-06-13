using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core.DataPresenters.WeaponDataPresenter
{
    public interface IWeaponDataPresenter
    {
        WeaponData GetWeaponData(WeaponType type, int context);
    }
}