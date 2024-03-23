using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitAttack : MonoBehaviour
    {
        [SerializeField] AudioClip _attackSFX;
        
        protected ITarget _target;
        protected IShootProxy _shootProxy;
        protected IVFXProxy _vFXProxy;

        private IAudioService _audioService;

        public void SetTarget(ITarget target)
        {
            _target = target;
        }

        public void SetShootProxy(IShootProxy shootProxy)
        {
            _shootProxy = shootProxy;
        }

        public void SetVFXProxy(IVFXProxy vFXProxy)
        {
            _vFXProxy = vFXProxy;
        }
        
        public virtual void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService)
        {
            _audioService = audioService;
        }

        //Animation event
        protected virtual void OnAttack()
        {
            if (_audioService != null && _attackSFX != null)
            {
                _audioService.PlayOneShot(_attackSFX);
            }   
        }
        
    }
}