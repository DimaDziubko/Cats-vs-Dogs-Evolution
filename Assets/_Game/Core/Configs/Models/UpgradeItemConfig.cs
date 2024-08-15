using System;

namespace _Game.Core.Configs.Models
{
    [Serializable]
    public class UpgradeItemConfig
    {
        public int Id;
        public float Price;
        public Exponential PriceExponential;
        public float Value;
        public float ValueStep;
    }
}