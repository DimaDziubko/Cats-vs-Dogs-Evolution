using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    [CreateAssetMenu(fileName = "EnemyAsset", menuName = "StaticData/Enemy asset")]
    public class BattleEnemyAsset : ScriptableObject
    {
        public EnemyAsset[] Assets;
    }
}