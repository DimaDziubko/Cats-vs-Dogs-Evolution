using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class AgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<WarriorConfig> Warriors;
        public List<int> WarriorsId;
        public string Name;
        public string AgeIconKey;
        public string Description;
        public string DateRange;
        public string BaseKey;
        public int Level;
    }
}