using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    [CreateAssetMenu(fileName = "UnitAsset", menuName = "StaticData/Age unit asset")]
    public class AgeUnitAsset : ScriptableObject
    {
       public UnitAsset[] Assets;
    }
}
