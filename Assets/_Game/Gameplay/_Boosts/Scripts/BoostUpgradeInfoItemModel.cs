using UnityEngine;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class BoostUpgradeInfoItemModel
    {
        public BoostType Type;
        public Sprite Icon;
        public float CurrentValue;
        public string DisplayValue;
        public string Delta;
        public bool IsUpgraded;
    }
}