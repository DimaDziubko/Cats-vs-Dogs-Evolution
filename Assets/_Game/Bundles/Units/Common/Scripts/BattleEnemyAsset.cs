using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    [CreateAssetMenu(fileName = "EnemyAsset", menuName = "StaticData/Enemy asset")]
    public class BattleEnemyAsset : ScriptableObject
    {
        public EnemyAsset[] Assets;
    }
}