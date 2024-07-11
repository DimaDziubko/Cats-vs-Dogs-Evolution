using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace Assets._Game.Scenes.Tests
{
    public class TestAttack : UnitAttack
    {
        [SerializeField] private AudioClip _sfx;
        [SerializeField] private ParticleSystem _muzzleFlash;

        [SerializeField] private AudioManager _audioManager;

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
        
            if (_audioManager!= null && _sfx != null)
            {
                _audioManager.PlayOneShot(_sfx);
            }
        }
    }
}
