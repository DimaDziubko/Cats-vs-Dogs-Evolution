using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Configs/Difficulty")]
    [Serializable]
    public class DifficultyConfig : ScriptableObject
    {
        public int Id;
        public Exponential DifficultyCurve;
        public List<float> InitialEvolutionPrices;
        public List<float> DeltaEvolutionPrices;
    }
}