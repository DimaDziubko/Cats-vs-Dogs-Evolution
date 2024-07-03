using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace Assets._Game.Scenes.Tests
{
    public class TestUnit : MonoBehaviour
    {
        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private Transform _target;
        private void Start()
        {
            _animator.Construct(0.8f);
            _animator.PlayAttack();
            _animator.SetTarget(_target);
        }
    }
}
