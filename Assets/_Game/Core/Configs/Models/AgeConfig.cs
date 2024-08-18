using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "AgeConfig", menuName = "Configs/Age")]
    [Serializable]
    public class AgeConfig : ScriptableObject
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<WarriorConfig> Warriors;
        public List<int> WarriorsId;
        public string Name;
        public string AgeIconKey;
        [MultiLineProperty(5)] 
        public string Description;
        public string DateRange;
        public AssetReferenceGameObject BasePrefab;
        public int Level;
    }
}