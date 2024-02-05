using _Game.Gameplay._Unit.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Unit._Target
{
    public class Target
    {
        public Transform Transform { get; private set; }
        public IDamageable Damageable { get; private set; }

        public Target(Transform transform, IDamageable damageable)
        {
            Transform = transform;
            Damageable = damageable;
        }
    }
}