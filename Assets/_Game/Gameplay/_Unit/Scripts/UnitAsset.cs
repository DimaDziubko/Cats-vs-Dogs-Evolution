using System;
using _Game.Gameplay.Common;
using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    [Serializable]
    public class UnitAsset
    {
        public int Id;
        public Sprite Icon;
        public Unit PlayerPrefab;
    }
}