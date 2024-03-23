using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderUI : MonoBehaviour
    {
        [SerializeField] private UnitBuildButton[] _buttons;
        
        public IEnumerable<UnitBuildButton> Buttons => _buttons;
        
    }
}