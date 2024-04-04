using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class TestUnit : MonoBehaviour
    {
        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private Transform _target;
        private void Start()
        {
            _animator.Construct();
            _animator.PlayAttack();
            _animator.SetTarget(_target);
        }
    }
}
