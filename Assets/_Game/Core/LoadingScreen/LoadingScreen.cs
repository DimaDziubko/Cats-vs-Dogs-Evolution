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
    public enum LoadingScreenType
    {
        Simple,
        Transparent,
        DarkFade,
    }
    
    [RequireComponent(typeof(Canvas))]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _progressFill;
        [SerializeField] private TextMeshProUGUI _loadingInfo;
        [SerializeField] private float _barSpeed;
        
        //Animation
        [SerializeField] private Image _splashImage;
        [SerializeField] private Image[] _fadableImages;

        private float _targetProgress;

        private void Awake() => 
            DontDestroyOnLoad(this);

        public async UniTask Load(
            Queue<ILoadingOperation> loadingOperations, LoadingScreenType type)
        {
            switch (type)
            {
                case LoadingScreenType.Simple:
                    await SimpleLoading(loadingOperations);
                    break;
                case LoadingScreenType.Transparent:
                    await TransparentLoading(loadingOperations);
                    break;
                case LoadingScreenType.DarkFade:
                    await DarkFadeLoading(loadingOperations);
                    break;
            }
        }

        private async UniTask TransparentLoading(Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = false;
            await LoadOperations(loadingOperations);
        }

        private async UniTask SimpleLoading(Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = true;
            _splashImage.color = Color.white;
            _loadingInfo.enabled = true;
            
            foreach (var image in _fadableImages)
            {
                image.enabled = true;
            }
            
            
            ResetFade(1);

            await LoadOperations(loadingOperations);
            await PlayFadeAnimation(1, 0,2);
        }

        private async UniTask DarkFadeLoading(Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = true;
            _splashImage.color = Color.black;
            _loadingInfo.enabled = false;
            
            foreach (var image in _fadableImages)
            {
                image.enabled = false;
            }
            
            ResetFade(0);

            var fadeTask = PlayFadeAnimation(0, 1, 2);
            var loadTask = LoadOperations(loadingOperations);

            await UniTask.WhenAll(fadeTask, loadTask);
            
            await PlayFadeAnimation(1, 0, 2);
        }


        private async UniTask LoadOperations(Queue<ILoadingOperation> loadingOperations)
        {
            foreach (var operation in loadingOperations)
            {
                ResetFill();
                _loadingInfo.text = operation.Description;
                await operation.Load(OnProgress);
                await WaitForBarFill();
            }
        }


        private async UniTask PlayFadeAnimation(float startValue, float endValue, float duration)
        {
            Sequence fadeSequence = DOTween.Sequence();

            foreach (var image in _fadableImages)
            {
                fadeSequence.Join(image.DOFade(endValue, duration).From(startValue));
            }

            fadeSequence.Join(_loadingInfo.DOFade(endValue, duration).From(startValue));
            
            fadeSequence.AppendInterval(0.15f);

            await fadeSequence.AsyncWaitForCompletion();
        }

        private void ResetFade(int value)
        {
            foreach (var image in _fadableImages)
            {
                var color = image.color;
                color = new Color(color.r, color.g, color.b, value);
                image.color = color;
            }
        
            _loadingInfo.color = new Color(_loadingInfo.color.r, _loadingInfo.color.g, _loadingInfo.color.b, value);
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
