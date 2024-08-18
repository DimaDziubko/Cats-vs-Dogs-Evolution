using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "GeneralWarriorsConfig", menuName = "Configs/Warriors")]
    public class GeneralWarriorsConfig : ScriptableObject
    {
        public List<WarriorConfig> WarriorConfigs;
    }
}