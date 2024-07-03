using Assets._Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.WeaponDataProviders
{
    public interface IWeaponDataProvider
    {
        UniTask<WeaponData> LoadWeapon(WeaponLoadOptions options);
    }
}