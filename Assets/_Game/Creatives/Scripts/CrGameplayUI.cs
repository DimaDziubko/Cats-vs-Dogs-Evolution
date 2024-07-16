using System.Collections;
using UnityEngine;

namespace Assets._Game.Creatives.Scripts
{
    public class CrGameplayUI : MonoBehaviour
    {
        [SerializeField] private CrUnitBuilButton[] _crUnitBuilButtons;

        public CrUnitBuilButton[] CrUnitBuilButtons => _crUnitBuilButtons;
    }
}