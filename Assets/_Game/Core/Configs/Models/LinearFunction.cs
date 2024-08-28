using UnityEngine;

namespace _Game.Core.Configs.Models
{
    public class LinearFunction
    {
        public int Id;
        public float A;
        public float B;

        public float GetIntValue(int level)
        {
            float result = A * level + B;
            return Mathf.Round(result);
        }
        
        public float GetValue(int level)
        {
            float result = A * level + B;
            return result;
        }
    }
}