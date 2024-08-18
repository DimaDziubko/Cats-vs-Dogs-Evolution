using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "GeneralBattlesConfig", menuName = "Configs/Battles")]
    public class GeneralBattlesConfig : ScriptableObject
    {
        public List<BattleConfig> BattleConfigs;
    }
}