using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._Bases.Scripts
{
    public class BaseDestructionAnimator : MonoBehaviour
    {
        public event Action AnimationCompleted; 
        
        [SerializeField] private Transform _baseBodyTransform;
        [SerializeField] private Transform _shadowTransform;
        
        [SerializeField] private Vector3 _destructionPosition;
        [SerializeField] private float _animationDuration = 3f;
        [SerializeField] private SpriteMask _destructionMask;
        
        public void Construct()
        {
            _destructionMask.enabled = false;
        }
        
        public void StartDestructionAnimation()
        {
            _destructionMask.enabled = true;

            _shadowTransform.DOScale(Vector3.zero, _animationDuration);
            _baseBodyTransform.DOLocalMove(_destructionPosition, _animationDuration)
                .OnStart(() =>
                {
                   
                })
                .OnComplete(() =>
                {
                    AnimationCompleted?.Invoke();
                });
        }
    }
}