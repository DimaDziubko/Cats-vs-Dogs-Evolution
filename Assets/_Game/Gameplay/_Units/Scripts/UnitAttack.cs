using System.Collections;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitAttack : MonoBehaviour
    {
        protected float DisableAttackDelay { get; set; } = 0.01f;

        [SerializeField] protected SoundData _soundData;

        [ShowInInspector, ReadOnly]
        protected ITarget _target;
        protected IShootProxy _shootProxy;
        protected IVFXProxy _vFXProxy;
        
        protected ISoundService _soundService;

        private Transform _unitTransform;

        protected bool _isActive;

        public IInteractionCache InteractionCache { get; set; }
        
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
            ISoundService soundService,
            Transform unitTransform)
        {
            _unitTransform = unitTransform;
            _soundService = soundService;
            _isActive = true;
        }

        public void Disable()
        {
            if(gameObject.activeInHierarchy)
                StartCoroutine(DisableAttackAfterDelay());
        }

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
            if(_target == null) return;
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
        
        protected void RotateToTarget(Vector3 destination)
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

        public virtual void SetPaused(in bool isPaused) { }
    }
}