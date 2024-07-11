using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class PersistentVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem.Stop();
        }

        //AnimationEvent
        public void ActivateVfx()
        {
            if(_particleSystem.isEmitting) return;
            _particleSystem.Play();
        }

        //AnimationEvent
        public void DeactivateVfx() => 
            _particleSystem.Stop();
        
    }
}
