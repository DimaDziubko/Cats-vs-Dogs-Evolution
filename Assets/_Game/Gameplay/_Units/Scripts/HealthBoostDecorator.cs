using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.Scripts
{
    public class HealthBoostDecorator : UnitDataDecorator
    {
        private readonly float _healthMultiplier;

        public HealthBoostDecorator(IUnitData unitData, float healthMultiplier)
            : base(unitData)
        {
            _healthMultiplier = healthMultiplier;
        }

        public override float GetUnitHealthForFaction(Faction faction)
        {
            var baseValue =  base.GetUnitHealthForFaction(faction);
            return baseValue * _healthMultiplier;
        }
        
        public override float GetStatBoost(StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return base.GetStatBoost(statType);
                case StatType.Health:
                    return _healthMultiplier;
                default:
                    return base.GetStatBoost(statType);
            }
        }
    }
}