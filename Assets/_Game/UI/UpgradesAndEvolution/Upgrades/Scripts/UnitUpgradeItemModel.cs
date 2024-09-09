using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UnitUpgradeItemModel
    {
        public UnitType Type;
        public Sprite WarriorIcon;
        public string Name;
        public float Price;
        public Dictionary<StatType, StatInfoModel> Stats;
        public bool IsBought;
        public ButtonState ButtonState;
    }
}