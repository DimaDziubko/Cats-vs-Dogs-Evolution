using System;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "CommonConfig", menuName = "Configs/Common")]
    [Serializable]
    public class CommonConfig : ScriptableObject
    {
        public int Id;
        public string FoodIconKey;
        public string CatFoodIconKey;
        public string DogFoodIconKey;
        public string BaseIconKey;
    }
}