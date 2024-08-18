using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "GeneralAgesConfig", menuName = "Configs/Ages")]
    public class GeneralAgesConfig : ScriptableObject
    {
        public List<AgeConfig> AgeConfigs;
    }
}