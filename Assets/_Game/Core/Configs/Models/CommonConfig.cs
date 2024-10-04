using System;
using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "CommonConfig", menuName = "Configs/Common")]
    [Serializable]
    public class CommonConfig : ScriptableObject
    {
        public int Id;
        public string FoodIconKey;
        public string CatFoodIconKey;
        public string DogFoodIconKey;
        public string BaseIconKey;

        [Space]
        public Sprite FoodIcon;
        public Sprite CatFoodIcon;
        public Sprite DogFoodIcon;
        public Sprite BaseIcon;
        
        [Space]
        public Sprite GemIcon;
        public Sprite CoinIcon;


        [FormerlySerializedAs("AllUnitAttackIcon")] [Space]
        public Sprite AllUnitDamageIcon;
        public Sprite AllUnitHealthIcon;
        public Sprite CoinsGainedIcon;
        public Sprite BaseHealthIcon;
        public Sprite FoodProductionIcon;


        [Space]
        public Sprite CatAttackIcon;
        public Sprite DogAttackIcon;
        
        [Space]
        public Sprite CatHealthIcon;
        public Sprite DogHealthIcon;
        
        
        public Sprite GetFoodIconForRace(Race race)
        {
            switch (race)
            {
                case Race.None:
                    return FoodIcon;
                case Race.Cat:
                    return CatFoodIcon;
                case Race.Dog:
                    return DogFoodIcon;
                default:
                    return FoodIcon;
            }
        }
        
        public Sprite GetAttackIconForRace(Race race)
        {
            switch (race)
            {
                case Race.None:
                    return CatAttackIcon;
                case Race.Cat:
                    return CatAttackIcon;
                case Race.Dog:
                    return DogAttackIcon;
                default:
                    return CatAttackIcon;
            }
        }

        public Sprite GetHealthIconForRace(Race race)
        {
            switch (race)
            {
                case Race.None:
                    return CatHealthIcon;
                case Race.Cat:
                    return CatHealthIcon;
                case Race.Dog:
                    return DogHealthIcon;
                default:
                    return CatHealthIcon;
            }
        }

        public Sprite GetBoostIconFor(BoostType boostType)
        {
            switch (boostType)
            {
                case BoostType.AllUnitDamage:
                    return AllUnitDamageIcon;
                case BoostType.AllUnitHealth:
                    return AllUnitHealthIcon;
                case BoostType.FoodProduction:
                    return FoodProductionIcon;
                case BoostType.BaseHealth:
                    return BaseHealthIcon;
                case BoostType.CoinsGained:
                    return CoinsGainedIcon;
                default:
                    throw new ArgumentOutOfRangeException(nameof(boostType), boostType, null);
            }
        }
    }
}