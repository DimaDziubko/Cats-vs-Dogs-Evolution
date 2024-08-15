using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "GeneralAgeConfig", menuName = "Configs/Ages")]
    [Serializable]
    public class GeneralAgeConfig : ScriptableObject
    {
        [FormerlySerializedAs("ageConfigs")] public List<AgeConfig> AgeConfigs;
    }
    
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
        public string BaseKey;
        public int Level;
    }
    
    public class RemoteAgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<int> WarriorsId;
    }
}