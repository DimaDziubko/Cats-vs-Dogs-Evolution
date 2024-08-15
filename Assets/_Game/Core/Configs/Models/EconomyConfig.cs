using System;

namespace _Game.Core.Configs.Models
{
    [Serializable]
    public class EconomyConfig
    {
        public int Id;
        public float CoinPerBattle;
        public int InitialFoodAmount;

        public UpgradeItemConfig FoodProduction;
        public UpgradeItemConfig BaseHealth;
    }
}