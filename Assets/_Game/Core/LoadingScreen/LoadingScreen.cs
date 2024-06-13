using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.LoadingScreen
{
    [RequireComponent(typeof(Canvas))]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _progressFill;
        [SerializeField] private TextMeshProUGUI _loadingInfo;
        [SerializeField] private float _barSpeed;
        
        //Animation
        [SerializeField] private Image[] _fadableImages;
        [SerializeField] private TMP_Text _fadableText;
        [SerializeField] private float _fadeAnimationDuration = 2f;
        
        private float _targetProgress;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public async UniTask Load(
            Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = true;
            ResetFade();
            
            StartCoroutine(UpdateProgressBar());
        
            foreach (var operation in loadingOperations)
            {
                ResetFill();
                _loadingInfo.text = operation.Description;
                await operation.Load(OnProgress);
                await WaitForBarFill();
            }
        }

        public async UniTask PlayFadeAnimation()
        {
            Sequence fadeSequence = DOTween.Sequence();

            foreach (var image in _fadableImages)
            {
                fadeSequence.Join(image.DOFade(0f, _fadeAnimationDuration).From(1f));
            }

            fadeSequence.Join(_fadableText.DOFade(0f, _fadeAnimationDuration).From(1f));
            
            fadeSequence.AppendInterval(0.15f);

            await fadeSequence.AsyncWaitForCompletion();
        }

        private void ResetFade()
        {
            foreach (var image in _fadableImages)
            {
                var color = image.color;
                color = new Color(color.r, color.g, color.b, 1f);
                image.color = color;
            }
        
            _fadableText.color = new Color(_fadableText.color.r, _fadableText.color.g, _fadableText.color.b, 1f);
        }
        
        private void ResetFill()
        {
            _progressFill.value = 0;
            _targetProgress = 0;
        }

        private void OnProgress(float progress)
        {
            _targetProgress = progress;
        }

        private async UniTask WaitForBarFill()
        {
            while (_progressFill.value < _targetProgress)
            {
                await UniTask.Yield();
            }
        
            await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
        }

        private IEnumerator UpdateProgressBar()
        {
            while (_canvas.enabled)
            {
                if(_progressFill.value < _targetProgress)
                    _progressFill.value += Time.deltaTime * _barSpeed;
                yield return null;
            }
        }
        
    }
}
