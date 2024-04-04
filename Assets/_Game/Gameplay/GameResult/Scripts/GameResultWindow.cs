using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Services.Audio;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.GameResult.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class GameResultWindow : MonoBehaviour
    {
        private const int REWARD_MULTIPLIER = 2;
        
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private GameResultIntroAnimation _introAnimation;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _adsButton;
        [SerializeField] private DoubleCoinsBtn _doubleCoinsBtn;
        
        [SerializeField] private Canvas _canvas;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IMyLogger _logger;
        private IAudioService _audioService;
        private IAdsService _adsService;

        private ICoinCounter _coinCounter;
        
        public void Construct(
            Camera uiCamera, 
            IAudioService audioService, 
            IMyLogger logger,
            IAdsService adsService)
        {
            _audioService = audioService;
            _adsService = adsService;
                
            _logger = logger;
            _canvas.worldCamera = uiCamera;
        }

        public async UniTask<bool> ShowAndAwaitForExit(ICoinCounter coinCounter, GameResultType result)
        {
            _quitButton.onClick.AddListener(OnQuitClicked);

            _doubleCoinsBtn.Initialize(OnAdsBtnClicked);

            _doubleCoinsBtn.SetInteractable(_adsService.IsRewardedVideoReady);

            _adsService.RewardedVideoLoaded += OnRewardedVideoLoaded;
            
            _coinCounter = coinCounter;
            
            _coinsLabel.text = coinCounter.Coins.FormatMoney();
            
            _taskCompletion = new UniTaskCompletionSource<bool>();
            
            _quitButton.interactable = false;
            _adsButton.interactable = false;
            
            _canvas.enabled = true;
            await _introAnimation.Play(result);

            _quitButton.interactable = true;
            _adsButton.interactable = _adsService.IsRewardedVideoReady;
            
            var isExit = await _taskCompletion.Task;
            return isExit;
        }

        private void OnRewardedVideoLoaded()
        {
            _doubleCoinsBtn.SetInteractable(_adsService.IsRewardedVideoReady);
        }

        private void OnQuitClicked()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
            _canvas.enabled = false;
            Cleanup();
        }

        private void OnAdsBtnClicked()
        {
            _audioService.PlayButtonSound();
            _adsService.ShowRewardedVideo(MultiplyRewardAndQuit);
        }

        private void MultiplyRewardAndQuit()
        {
            _coinCounter.MultiplyCoins(REWARD_MULTIPLIER);
            OnQuitClicked();
        }

        private void Cleanup()
        {
            _adsService.RewardedVideoLoaded -= OnRewardedVideoLoaded;
            _quitButton.onClick.RemoveAllListeners();
            _doubleCoinsBtn.Cleanup();
        }
    }
}