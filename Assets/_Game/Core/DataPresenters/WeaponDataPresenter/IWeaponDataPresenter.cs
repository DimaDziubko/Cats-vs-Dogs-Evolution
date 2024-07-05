using Assets._Game.Gameplay._Weapon.Scripts;

namespace Assets._Game.Core.DataPresenters.WeaponDataPresenter
{
    public interface IWeaponDataPresenter
    {
        WeaponData GetWeaponData(WeaponType type, int context);
    }
}