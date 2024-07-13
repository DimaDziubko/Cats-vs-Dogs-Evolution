using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class TargetPoint : MonoBehaviour, ITarget
    {
        public IDamageable Damageable { get; set; }
        public Transform Transform { get; set; }
        public bool IsActive => !Damageable.IsDead;
    }
}