using System;
using _Game.Core.Configs.Models;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Gameplay._Units.Scripts
{
    [Serializable]
    public class UnitData
    {
        public WarriorConfig Config;
        public Unit Prefab;
        public int UnitLayer;
        public int AggroLayer;
        public int AttackLayer;
        public Race Race;
    }
}