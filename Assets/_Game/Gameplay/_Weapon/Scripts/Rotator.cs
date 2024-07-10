using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
        [SerializeField] private bool _clockwise = true;

        public void Rotate()
        {
            float direction = _clockwise ? 1f : -1f;
            _transform.Rotate(_rotationAxis * _rotationSpeed * direction * Time.deltaTime, Space.Self);
        }
    }
}