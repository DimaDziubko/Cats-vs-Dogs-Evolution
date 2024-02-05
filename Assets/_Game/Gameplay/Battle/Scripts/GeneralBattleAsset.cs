using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Gameplay.Battle.Scripts
{
    [CreateAssetMenu(fileName = "BattleAsset", menuName = "StaticData/Battle Asset")]
    public class GeneralBattleAsset : ScriptableObject
    {
        [FormerlySerializedAs("Configs")] public BattleAsset[] Assets;
    }
}