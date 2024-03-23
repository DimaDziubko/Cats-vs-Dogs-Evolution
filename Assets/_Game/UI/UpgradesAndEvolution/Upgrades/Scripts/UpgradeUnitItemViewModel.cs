using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradeUnitItemViewModel
    {
        public UnitType Type;
        public Sprite Icon;
        public string Name;
        public float Price;
        public bool IsBought;
        public bool CanAfford;
    }
}