using System;
using _Game.Core.Configs.Models;
using Sirenix.OdinInspector;

namespace _Game.Gameplay._Boosts.Scripts
{
    [Serializable]
    public class Boost
    {
        [ValueDropdown("GetBoostTypes")]
        [OnValueChanged("UpdateNameBasedOnType")]
        public BoostType Type;
        
        [ReadOnly]
        public string Name;
        public Exponential Exponential;
        
        private void UpdateNameBasedOnType() => Name = GetNameForType(Type);
        private string GetNameForType(BoostType type)
        {
            switch (type)
            {
                case BoostType.AllUnitDamage:
                    return "All Unit Damage";
                case BoostType.AllUnitHealth:
                    return "All Unit Health";
                case BoostType.FoodProduction:
                    return "Food Production";
                case BoostType.BaseHealth:
                    return "Base Health";
                case BoostType.CoinsGained:
                    return "Coins Gained";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private BoostType[] GetBoostTypes() => (BoostType[])System.Enum.GetValues(typeof(BoostType));
        
    }
}