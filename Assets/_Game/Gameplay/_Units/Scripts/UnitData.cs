using System;
using Assets._Game.Core.Configs.Models;

namespace Assets._Game.Gameplay._Units.Scripts
{
    [Serializable]
    public class UnitData
    {
        public WarriorConfig Config;
        public Unit Prefab;
        public int UnitLayer;
        public int AggroLayer;
        public int AttackLayer;
    }
}