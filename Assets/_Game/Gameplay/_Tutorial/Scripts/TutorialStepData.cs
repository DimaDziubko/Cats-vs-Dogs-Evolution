using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialStepData
    {
        public int Step;
        public int[] AffectedSteps;
        public Vector2 RequiredPointerSize;
        public Vector3 RequiredPointerPosition;
        public Quaternion RequiredPointerRotation;
        public bool NeedAppearanceAnimation;
        public bool IsUnderneath;
    }
}