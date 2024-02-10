using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    [CreateAssetMenu(fileName = "UnitAsset", menuName = "StaticData/Age unit asset")]
    public class AgeUnitAsset : ScriptableObject
    {
       public UnitAsset[] Assets;
    }
}
