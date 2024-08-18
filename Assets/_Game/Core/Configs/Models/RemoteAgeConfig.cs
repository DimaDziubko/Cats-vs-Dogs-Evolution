using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class RemoteAgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<int> WarriorsId;
    }
}