using System;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    [Serializable]
    public class UnitAsset
    {
        public int Id;
        public Sprite Icon;
        public Unit PlayerPrefab;
    }
}