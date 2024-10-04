using _Game.UI.UpgradesAndEvolution.Scripts;

namespace _Game.Gameplay._Units.Scripts
{
    public class DamageBoostDecorator : UnitDataDecorator
    {
        private readonly float _damageMultiplier;

        public DamageBoostDecorator(IUnitData unitData, float damageMultiplier)
            : base(unitData)
        {
            _damageMultiplier = damageMultiplier;
        }

        public override float Damage => base.Damage * _damageMultiplier;

        public override float GetStatBoost(StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return _damageMultiplier;
                case StatType.Health:
                    return base.GetStatBoost(statType);
                default:
                    return base.GetStatBoost(statType);
            }
        }
    }
}