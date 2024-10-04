using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class WeaponDataDamageBoostDecorator : WeaponDataDecorator
    {
        private readonly float _damageMultiplier;

        public WeaponDataDamageBoostDecorator(IWeaponData weaponData, float damageMultiplier) : base(weaponData)
        {
            _damageMultiplier = damageMultiplier;
        }

        public override float GetProjectileDamageForFaction(Faction faction)
        {
            var baseValue =  base.GetProjectileDamageForFaction(faction);
            return baseValue * _damageMultiplier;
        }
    }
}