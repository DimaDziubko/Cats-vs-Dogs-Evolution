namespace Assets._Game.Core.Ads
{
    // public class UnityAdsService : IAdsService, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    // {
    //     private const string ANDROID_GAME_ID = "5584836";
    //     private const string IOS_GAME_ID = "5584837";
    //
    //     private const string REWARDED_VIDEO_PLACEMENT_ID = "myRewardedVideo";
    //     
    //     private string _gameId;
    //     private Action _onVideoCompleted;
    //     private bool _testMode = true;
    //     private bool _isRewardedVideoReady = false;
    //
    //     public event Action RewardedVideoLoaded;
    //
    //     private readonly IPauseManager _pauseManager;
    //     private readonly IMyLogger _logger;
    //
    //     public bool IsRewardedVideoReady => _isRewardedVideoReady && IsInternetConnected();
    //
    //     public void Init()
    //     {
    //         _gameId = Application.platform switch
    //         {
    //             RuntimePlatform.Android => ANDROID_GAME_ID,
    //             RuntimePlatform.IPhonePlayer => IOS_GAME_ID,
    //             RuntimePlatform.WindowsEditor => ANDROID_GAME_ID,
    //             _ => throw new NotSupportedException("Unsupported platform for ads")
    //         };
    //
    //         if (!Advertisement.isInitialized && Advertisement.isSupported)
    //         {
    //             Advertisement.Initialize(_gameId, _testMode, this);
    //         }
    //     }
    //
    //     public UnityAdsService(
    //         IPauseManager pauseManager,
    //         IMyLogger logger)
    //     {
    //         _pauseManager = pauseManager;
    //         _logger = logger;
    //     }
    //
    //     public void ShowRewardedVideo(Action onVideoCompleted)
    //     {
    //         if (!IsRewardedVideoReady)
    //         {
    //             _logger.LogWarning("Attempted to show rewarded video before it was ready.");
    //             return;
    //         }
    //
    //         _pauseManager.SetPaused(true);
    //         _onVideoCompleted = onVideoCompleted;
    //         Advertisement.Show(REWARDED_VIDEO_PLACEMENT_ID, this);
    //     }
    //
    //     public void OnInitializationComplete()
    //     {
    //         _logger.Log("Unity Ads initialization complete.");
    //         LoadAd();
    //     }
    //
    //     public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    //     {
    //         _isRewardedVideoReady = false;
    //         _logger.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    //     }
    //
    //     public void OnUnityAdsAdLoaded(string placementId)
    //     {
    //         if (placementId == REWARDED_VIDEO_PLACEMENT_ID)
    //         {
    //             _isRewardedVideoReady = true;
    //             RewardedVideoLoaded?.Invoke();
    //         }
    //     }
    //
    //     public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) => 
    //         _logger.LogError($"OnUnityAdsFailedToLoad {placementId} {error} {message}");
    //
    //     public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) => 
    //         _logger.LogError($"OnUnityAdsShowFailure {placementId} {error} {message}");
    //
    //     public void OnUnityAdsShowStart(string placementId) => 
    //         _logger.Log($"OnUnityAdsShowStart {placementId}");
    //
    //     public void OnUnityAdsShowClick(string placementId) => 
    //         _logger.Log($"OnUnityAdsShowClick {placementId}");
    //
    //     public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    //     {
    //         if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
    //         
    //         switch (showCompletionState)
    //         {
    //             case UnityAdsShowCompletionState.SKIPPED:
    //                 _logger.LogWarning($"OnUnityAdsShowComplete {placementId} {showCompletionState}");
    //                 break;
    //             case UnityAdsShowCompletionState.COMPLETED:
    //                 _onVideoCompleted?.Invoke();
    //                 break;
    //             case UnityAdsShowCompletionState.UNKNOWN:
    //                 _logger.LogError($"OnUnityAdsShowComplete {placementId} {showCompletionState}");
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException(nameof(showCompletionState), showCompletionState, null);
    //         }
    //         
    //         _onVideoCompleted = null;
    //
    //         LoadAd();
    //     }
    //
    //     private void LoadAd()
    //     {
    //         if (!Advertisement.isInitialized)
    //         {
    //             _logger.LogWarning("Attempting to load ad before initialization.");
    //             return;
    //         }
    //     
    //         Advertisement.Load(REWARDED_VIDEO_PLACEMENT_ID, this);
    //     }
    //     
    //     private bool IsInternetConnected()
    //     {
    //         return Application.internetReachability != NetworkReachability.NotReachable;
    //     }
    // }
}