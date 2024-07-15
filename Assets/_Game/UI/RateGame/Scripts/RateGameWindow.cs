using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif

namespace Assets._Game.UI.RateGame.Scripts
{
    public class RateGameWindow : MonoBehaviour
    {

        private const string PP_RATE_GAME_CLICKED = "isrategameclicked_save";

        public event Action OnClose;
        private event Action OnRateGame;

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


        private void OnEnable()
        {
            OnClose += Hide;
            OnRateGame += RateGame;

            _noCloseButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                PlayButtonSound();
            });

            _rateGameButton.onClick.AddListener(() =>
            {
                OnRateGame?.Invoke();
                PlayButtonSound();
            });
        }

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
        }

        public void Show()
        {
            Unsubscribe();
            Subscribe();

            if (PlayerPrefs.HasKey(PP_RATE_GAME_CLICKED))
                return;

            _canvas.enabled = true;
        }

        private void Subscribe()
        {

        }

        private void Unsubscribe()
        {

        }

        public void Hide()
        {
            Unsubscribe();
            _canvas.enabled = false;
        }


        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();


        private void RateGame()
        {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
            StartCoroutine(LaunchReview());
#endif
            Invoke(nameof(Hide), 1f);

            PlayerPrefs.SetInt(PP_RATE_GAME_CLICKED, 1);
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
        }
#endif
        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }


        private void OnDisable()
        {
            OnClose -= Hide;
            OnRateGame -= RateGame;

            _noCloseButton.onClick.RemoveAllListeners();

            _rateGameButton.onClick.RemoveAllListeners();
        }
    }
}