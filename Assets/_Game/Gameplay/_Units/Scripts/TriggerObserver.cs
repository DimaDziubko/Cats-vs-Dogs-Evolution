using System;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObserver : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _collider;
        public event Action<Collider2D> TriggerEnter;
        public event Action<Collider2D> TriggerExit;

        public void Construct(in int layer)
        {
            gameObject.layer = layer;
        }

        public void SetSize(float radius)
        {
            if (_collider != null)
            {
                _collider.radius = radius;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) => 
            TriggerEnter?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) => 
            TriggerExit?.Invoke(other);
    }
}
