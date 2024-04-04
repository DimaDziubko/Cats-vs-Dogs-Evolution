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
        
        private Transform _unitTransform;

        private Vector3 Position
        {
            get => _unitTransform.position;
            set => _unitTransform.position = value;
        }
        private Quaternion Rotation
        {
            get => _unitTransform.rotation;
            set => _unitTransform.rotation = value;
        }
        
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
            IAudioService audioService,
            Transform unitTransform)
        {
            _audioService = audioService;
            _unitTransform = unitTransform;
        }

        //Animation event
        protected virtual void OnAttack()
        {
            RotateToTarget(_target.Transform.position);
            
            if (_audioService != null && _attackSFX != null)
            {
                _audioService.PlayOneShot(_attackSFX);
            }   
        }
        
        private void RotateToTarget(Vector3 destination)
        {
            Rotation = Quaternion.Euler(0, destination.x < Position.x ? 180 : 0, 0);
        }
    }
}