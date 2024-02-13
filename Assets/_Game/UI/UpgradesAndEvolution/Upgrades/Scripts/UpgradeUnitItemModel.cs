using _Game.Bundles.Units.Common.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItemModel
    {
        public UnitType Type;
        public Sprite Icon;
        public string Name;
        public float Price;
        public bool IsBought;
        public bool CanAfford;
    }
}