﻿using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class ImageFlashEffect : MonoBehaviour
    {
        private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
        
        [SerializeField] private Image[] _images;
        [SerializeField] private Material[] _materials;
        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        
        [SerializeField] private float _flashTime = 0.25f;
        [SerializeField] private AnimationCurve _flashSpeedCurve;
        
        
        private Coroutine _imageFlashCoroutine;

        public void Init(Color? optionalFlashColor = null)
        {
            if (optionalFlashColor != null)
            {
                _flashColor = optionalFlashColor.Value * 5;
            }
            
            _materials = new Material[_images.Length];
            
            for (int i = 0; i < _images.Length; i++)
            {
                _materials[i] = new Material(_images[i].material);
                _images[i].material = _materials[i];
            }
        }

        public void Reset()
        {
            SetFlashAmount(0);
        }

        
        public void TriggerFlash(Action callback = null)
        {
            if (_imageFlashCoroutine != null)
            {
                StopCoroutine(_imageFlashCoroutine);
            }

            _imageFlashCoroutine = StartCoroutine(ImageFlasher(callback));
        }

        private IEnumerator ImageFlasher(Action callback = null)
        {
            SetFlashColor();
            
            float elapsedTime = 0f;

            while (elapsedTime < _flashTime)
            {
                elapsedTime += Time.deltaTime;
                var currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _flashTime));
                
                SetFlashAmount(currentFlashAmount);
                
                yield return null;
            }
            
            callback?.Invoke();
        }
        
        private void SetFlashAmount(float amount)
        {
            foreach (var t in _materials)
            {
                t.SetFloat(FlashAmount, amount);
            }
        }
        
        private void SetFlashColor()
        {
            foreach (var t in _materials)
            {
                t.SetColor(FlashColor, _flashColor);
            }
        }
        
#if UNITY_EDITOR

        [Button]
        private void ManualInit()
        {
            _images = GetComponentsInChildren<Image>();
            
        }
#endif
    }
}