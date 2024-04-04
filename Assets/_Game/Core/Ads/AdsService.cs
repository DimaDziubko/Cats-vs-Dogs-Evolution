using System;
using _Game.Core.Pause.Scripts;
using UnityEngine;
using UnityEngine.Advertisements;

namespace _Game.Core.Ads
{
    public class AdsService : IAdsService, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    {
        private const string ANDROID_GAME_ID = "5584836";
        private const string IOS_GAME_ID = "5584837";

        private const string REWARDED_VIDEO_PLACEMENT_ID = "myRewardedVideo";
        
        private string _gameId;
        private Action _onVideoCompleted;
        private bool _testMode = true;
        private bool _isRewardedVideoReady = false;

        public event Action RewardedVideoLoaded;

        private readonly IPauseManager _pauseManager;

        public bool IsRewardedVideoReady => _isRewardedVideoReady;
        
        public void Init()
        {
            _gameId = Application.platform switch
            {
                RuntimePlatform.Android => ANDROID_GAME_ID,
                RuntimePlatform.IPhonePlayer => IOS_GAME_ID,
                RuntimePlatform.WindowsEditor => ANDROID_GAME_ID,
                _ => throw new NotSupportedException("Unsupported platform for ads")
            };

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        }

        public AdsService(IPauseManager pauseManager)
        {
            _pauseManager = pauseManager;
        }

        public void ShowRewardedVideo(Action onVideoCompleted)
        {
            if (!IsRewardedVideoReady)
            {
                Debug.LogWarning("Attempted to show rewarded video before it was ready.");
                return;
            }

            _onVideoCompleted = onVideoCompleted;
            Advertisement.Show(REWARDED_VIDEO_PLACEMENT_ID, this);
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
            LoadAd();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            _isRewardedVideoReady = false;
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId == REWARDED_VIDEO_PLACEMENT_ID)
            {
                _isRewardedVideoReady = true;
                RewardedVideoLoaded?.Invoke();
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) => 
            Debug.LogError($"OnUnityAdsFailedToLoad {placementId} {error} {message}");

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) => 
            Debug.LogError($"OnUnityAdsShowFailure {placementId} {error} {message}");

        public void OnUnityAdsShowStart(string placementId) => 
            Debug.Log($"OnUnityAdsShowStart {placementId}");

        public void OnUnityAdsShowClick(string placementId) => 
            Debug.Log($"OnUnityAdsShowClick {placementId}");

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
            
            switch (showCompletionState)
            {
                case UnityAdsShowCompletionState.SKIPPED:
                    Debug.LogWarning($"OnUnityAdsShowComplete {placementId} {showCompletionState}");
                    break;
                case UnityAdsShowCompletionState.COMPLETED:
                    _onVideoCompleted?.Invoke();
                    break;
                case UnityAdsShowCompletionState.UNKNOWN:
                    Debug.LogError($"OnUnityAdsShowComplete {placementId} {showCompletionState}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(showCompletionState), showCompletionState, null);
            }
            
            _onVideoCompleted = null;

            LoadAd();
        }

        private void LoadAd()
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogWarning("Attempting to load ad before initialization.");
                return;
            }
        
            Advertisement.Load(REWARDED_VIDEO_PLACEMENT_ID, this);
        }
    }
}