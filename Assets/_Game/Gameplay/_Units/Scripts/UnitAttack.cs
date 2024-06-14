using System.Collections;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitAttack : MonoBehaviour
    {
        protected float DisableAttackDelay { get; set; } = 0.1f;

        [SerializeField] private SoundData _soundData;

        protected ITarget _target;
        protected IShootProxy _shootProxy;
        protected IVFXProxy _vFXProxy;

        private IAudioService _audioService;
        private ISoundService _soundService;

        private Transform _unitTransform;

        protected bool _isActive;

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
        
        public void SetTarget(ITarget target) => 
            _target = target;

        public void SetShootProxy(IShootProxy shootProxy) => 
            _shootProxy = shootProxy;

        public void SetVFXProxy(IVFXProxy vFXProxy) => 
            _vFXProxy = vFXProxy;

        public virtual void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService,
            ISoundService soundService,
            Transform unitTransform)
        {
            _audioService = audioService;
            _unitTransform = unitTransform;
            _soundService = soundService;
            _isActive = true;
        }

        public void Disable() => 
            StartCoroutine(DisableAttackAfterDelay());

        IEnumerator DisableAttackAfterDelay()
        {
            yield return new WaitForSeconds(DisableAttackDelay);
            _isActive = false;
        }
        
        public void Enable() => 
            _isActive = true;

        //Animation event
        protected virtual void OnAttack()
        {
            RotateToTarget(_target.Transform.position);
            
            if (_soundService != null && _soundData != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_soundData)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }

        private void RotateToTarget(Vector3 destination)
        {
            if (destination.x < Position.x - Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (destination.x > Position.x + Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}