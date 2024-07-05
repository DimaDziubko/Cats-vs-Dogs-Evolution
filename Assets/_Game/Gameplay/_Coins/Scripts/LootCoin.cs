using System;
using Assets._Game.Utils;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Game.Gameplay._Coins.Scripts
{
    public class LootCoin : Coin
    {
        public event Action<LootCoin> AnimationCompleted;
        public float Amount { get; private set; }
        
        //Animation data
        [SerializeField, FloatRangeSlider(1.2f, 1.7f)]
        private FloatRange _jumpDistance = new FloatRange(1.2f, 1.7f);
        
        [SerializeField, FloatRangeSlider(0.3f, 0.6f)]
        private FloatRange _jumpPower = new FloatRange(0.3f, 0.6f);

        [SerializeField] private float[] _directionAngles;
        
        [SerializeField, FloatRangeSlider(-10f, 10f)] 
        private FloatRange _directionDeviation = new FloatRange(-10f, 10f);
        
        [SerializeField] private int _numJumps = 2;
        [SerializeField] private float _jumpDuration = 1f;
        
        [SerializeField] private float _scaleDuration = 1f;

        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private float _moveDelay = 1f;
        
        private Vector3 _targetPoint;

        public void Init(float amount, Vector3 targetPoint)
        {
            Amount = amount;   
            _transform.localScale = Vector3.zero;
           _targetPoint = targetPoint;
        }

        [Button]
        public void Jump()
        {
            float randomDirectionAngle = _directionAngles[Random.Range(0, _directionAngles.Length)];

            float deviatedAngle = randomDirectionAngle + _directionDeviation.RandomValueInRange;
            
            float angleInRadians = deviatedAngle * Mathf.PI / 180f;

            float deltaX = Mathf.Cos(angleInRadians);
            float deltaY = Mathf.Sin(angleInRadians);
            
            
            Vector3 finalJumpPosition = Position + new Vector3(deltaX, deltaY, 0).normalized * _jumpDistance.RandomValueInRange;
            
            _transform.DOScale(Vector3.one, _scaleDuration);
        
            _transform.DOJump(finalJumpPosition, _jumpPower.RandomValueInRange, _numJumps, _jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
            {
                DOVirtual.DelayedCall(_moveDelay, MoveToTarget);
            });
        }

        private void MoveToTarget()
        {
            _transform.DOMove(_targetPoint, _moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
            {
                AnimationCompleted?.Invoke(this);
                Recycle();
            });
        }
    }
}