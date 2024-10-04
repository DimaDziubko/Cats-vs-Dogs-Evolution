using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core.Data.Age.Static
{
    public class AgeStaticData
    {
        public DataPool<UnitType, IUnitData> UnitDataPool { get; set; }
        public DataPool<int, WeaponData> WeaponDataPool { get; set; }
        public BaseStaticData BaseStaticData { get; set; }
        public IUnitData ForUnit(UnitType type) => UnitDataPool.ForType(type);
        public WeaponData ForWeapon(int weaponId) => WeaponDataPool.ForType(weaponId);
        public BaseStaticData ForBase() => BaseStaticData;

        public void Cleanup()
        {
            UnitDataPool.Cleanup();
            WeaponDataPool.Cleanup();
            BaseStaticData = null;
        }

    }
}