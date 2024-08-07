namespace _Game.Core.Configs.Models
{
    public class EconomyConfig
    {
        public int Id;
        public float CoinPerBattle;
        public int InitialFoodAmount;

        public UpgradeItemConfig FoodProduction;
        public UpgradeItemConfig BaseHealth;
    }
}