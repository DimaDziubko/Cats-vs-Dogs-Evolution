using System.Collections.Generic;
using Assets._Game.Gameplay._Tutorial.Scripts;
using UnityEngine;

namespace Assets._Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderUI : MonoBehaviour
    {
        [SerializeField] private UnitBuildButton[] _buttons;
        [SerializeField] private TutorialStep _tutorialStep;
        public IEnumerable<UnitBuildButton> Buttons => _buttons;
        public TutorialStep TutorialStep => _tutorialStep;
    }
}