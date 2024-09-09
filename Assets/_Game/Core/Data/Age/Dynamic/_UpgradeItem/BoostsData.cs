using System;
using _Game.Gameplay._Boosts.Scripts;

namespace _Game.Core.Data.Age.Dynamic._UpgradeItem
{
    public class BoostsData : IBoostsDataReadonly
    {
        public event Action<BoostSource, BoostType, float> Changed;
        
        public float CardsAllUnitsDamageBoost;
        public float CardsAllUnitsHealthBoost;
        public float CardsFoodProductionBoost;
        public float CardsCoinsGainedBoost;
        public float CardsBaseHealthBoost;

        public void Change(BoostSource source, BoostType boostType, float value)
        {
            switch (source)
            {
                case BoostSource.TotalBoosts:
                    break;
                case BoostSource.Cards:
                    ChangeCardsBoosts(boostType, value);
                    break;
            }
            
            Changed?.Invoke(source, boostType, value);
        }

        float IBoostsDataReadonly.GetBoost(BoostSource source, BoostType type)
        {
            switch (source)
            {
                case BoostSource.TotalBoosts:
                    return GetTotalBoost(type);
                case BoostSource.Cards:
                    return GetCardBoost(type);
                default:
                    return 1;
            }
        }

        private void ChangeCardsBoosts(BoostType boostType, float value)
        {
            switch (boostType)
            {
                case BoostType.None:
                    return; 
                case BoostType.AllUnitDamage:
                    CardsAllUnitsDamageBoost = value;
                    break;
                case BoostType.AllUnitHealth:
                    CardsAllUnitsHealthBoost = value;
                    break;
                case BoostType.FoodProduction:
                    CardsFoodProductionBoost = value;
                    break;
                case BoostType.BaseHealth:
                    CardsBaseHealthBoost = value;
                    break;
                case BoostType.CoinsGained:
                    CardsCoinsGainedBoost = value;
                    break;
                default:
                    return;
            }
        }

        private float GetCardBoost(BoostType type)
        {
            switch (type)
            {
                case BoostType.None:
                    return 1;
                case BoostType.AllUnitDamage:
                    return CardsAllUnitsDamageBoost;
                case BoostType.AllUnitHealth:
                    return CardsAllUnitsHealthBoost;
                case BoostType.FoodProduction:
                    return CardsFoodProductionBoost;
                case BoostType.BaseHealth:
                    return CardsBaseHealthBoost;
                case BoostType.CoinsGained:
                    return CardsCoinsGainedBoost;
                default:
                    return 1;
            }
        }

        private float GetTotalBoost(BoostType type)
        {
            switch (type)
            {
                case BoostType.None:
                    return 1;
                case BoostType.AllUnitDamage:
                    return CardsAllUnitsDamageBoost;
                case BoostType.AllUnitHealth:
                    return CardsAllUnitsHealthBoost;
                case BoostType.FoodProduction:
                    return CardsFoodProductionBoost;
                case BoostType.BaseHealth:
                    return CardsBaseHealthBoost;
                case BoostType.CoinsGained:
                    return CardsCoinsGainedBoost;
                default:
                    return 1;
            }
        }
    }
}