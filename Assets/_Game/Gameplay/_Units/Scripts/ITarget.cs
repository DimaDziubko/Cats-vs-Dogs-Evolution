using UnityEngine;

namespace Assets._Game.Gameplay._Units.Scripts
{
    public interface ITarget
    { 
        IDamageable Damageable { get; }
        Transform Transform { get; }
        bool IsActive { get;}
    }
}