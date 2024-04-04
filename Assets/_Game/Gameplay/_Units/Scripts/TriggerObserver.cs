using System;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObserver : MonoBehaviour
    {
        public event Action<Collider2D> TriggerEnter;
        public event Action<Collider2D> TriggerExit;

        public void Construct(in int layer)
        {
            gameObject.layer = layer;
        }

        private void OnTriggerEnter2D(Collider2D other) => 
            TriggerEnter?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) => 
            TriggerExit?.Invoke(other);
    }
}
