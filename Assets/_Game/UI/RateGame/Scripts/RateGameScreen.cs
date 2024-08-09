#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif
using System;
using System.Collections;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.RateGame.Scripts
{
    public class RateGameScreen : MonoBehaviour
    {
        public event Action OnClose;
        public event Action OnRateGame;
        public event Action OnSetPP;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _rateGameButton;
        [SerializeField] private Button _noCloseButton;


#if UNITY_ANDROID
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
        private Coroutine _coroutineInit;
#endif

        private IAudioService _audioService;
        private IMyLogger _logger;

        private UniTaskCompletionSource<bool> _taskCompletion;


        private void Start()
        {
#if UNITY_ANDROID
            _coroutineInit = StartCoroutine(InitReview());
#endif
        }

#if UNITY_ANDROID
        private IEnumerator InitReview(bool force = false)
        {
            if (_reviewManager == null) _reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                if (force) DirectlyOpen();
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();
        }
#endif

        public void Construct(IWorldCameraService cameraService, IAudioService audioService, IMyLogger logger)
        {
            _logger = logger;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _audioService = audioService;
            Unsubscribe();
            Subscribe();
        }

        public async UniTask<bool> AwaitForDecision()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void Subscribe()
        {
            OnClose += Close;
            OnRateGame += RateGame;

            _noCloseButton.onClick.AddListener(() =>
            {
                OnSetPP?.Invoke();
                OnClose?.Invoke();
                PlayButtonSound();
            });

            _rateGameButton.onClick.AddListener(() =>
            {
                OnSetPP?.Invoke();
                OnRateGame?.Invoke();
                PlayButtonSound();
            });
        }

        private void Unsubscribe()
        {
            OnClose -= Close;
            OnRateGame -= RateGame;

            _noCloseButton.onClick.RemoveAllListeners();
            _rateGameButton.onClick.RemoveAllListeners();
        }

        private void Close() => _taskCompletion.TrySetResult(true);


        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();


        private void RateGame()
        {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
            StartCoroutine(LaunchReview());
#endif
            Invoke(nameof(Close), 1f);
        }


#if UNITY_ANDROID
        public IEnumerator LaunchReview()
        {
            if (_playReviewInfo == null)
            {
                if (_coroutineInit != null) StopCoroutine(_coroutineInit);
                yield return StartCoroutine(InitReview(true));
            }

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null;
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                DirectlyOpen();
                yield break;
            }

            _taskCompletion.TrySetResult(true);
        }
#endif
        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }

    }
}