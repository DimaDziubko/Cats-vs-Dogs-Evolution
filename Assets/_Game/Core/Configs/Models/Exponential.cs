using System;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [Serializable]
    public class Exponential
    {
        public int Id;
        public float A;
        public float B;
        public float C;
        public float D;

        public float GetValue(int level)
        {
            float cost = A * Mathf.Exp(B + C * level) + D;
            return cost;
        }
    }
}