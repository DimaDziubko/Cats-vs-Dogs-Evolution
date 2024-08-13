using _Game.Common;
using _Game.Core.Ads;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay.GameResult.Scripts;
using CAS;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.GameResult.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class GameResultPopup : MonoBehaviour
    {
        private const int REWARD_MULTIPLIER = 2;

        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private GameResultIntroAnimation _introAnimation;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _adsButton;
        [SerializeField] private DoubleCoinsBtn _doubleCoinsBtn;
        [SerializeField] private TMP_Text _victoryText;

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

        public async UniTask<bool> ShowAndAwaitForExit(
            ICoinCounter coinCounter,
            GameResultType result)
        {
            _victoryText.enabled = false;

            if (result == GameResultType.Victory)
            {
                _victoryText.enabled = true;
                _audioService.PlayVictorySound();
            }

            _quitButton.onClick.AddListener(OnQuitClicked);

            _doubleCoinsBtn.Initialize(OnAdsBtnClicked);
            _doubleCoinsBtn.SetInteractable(_adsService.IsAdReady(AdType.Rewarded));

            _adsService.VideoLoaded += OnVideoLoaded;

            _coinCounter = coinCounter;

            _coinsLabel.text = coinCounter.Coins.FormatMoney();

            _taskCompletion = new UniTaskCompletionSource<bool>();

            _quitButton.interactable = false;
            _adsButton.interactable = false;

            _canvas.enabled = true;
            await _introAnimation.Play(result);

            _quitButton.interactable = true;
            _adsButton.interactable = _adsService.IsAdReady(AdType.Rewarded);

            var isExit = await _taskCompletion.Task;
            return isExit;
        }

        private void OnVideoLoaded(AdType type)
        {
            if (type == AdType.Rewarded)
                _doubleCoinsBtn.SetInteractable(_adsService.IsAdReady(AdType.Rewarded));
        }

        private void OnQuitClicked()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
            _canvas.enabled = false;
            Cleanup();
            if (_adsService.IsAdReady(AdType.Interstitial))
                _adsService.ShowInterstitialVideo(Placement.GameResult);
            else
                _logger.Log("Inter_ Not Ready");
        }

        private void OnAdsBtnClicked()
        {
            _audioService.PlayButtonSound();
            _adsService.ShowRewardedVideo(MultiplyRewardAndQuit, Placement.X2);
        }

        private void MultiplyRewardAndQuit()
        {
            _coinCounter.MultiplyCoins(REWARD_MULTIPLIER);
            OnQuitClicked();
        }

        private void Cleanup()
        {
            _adsService.VideoLoaded -= OnVideoLoaded;
            _quitButton.onClick.RemoveAllListeners();
            _doubleCoinsBtn.Cleanup();
        }
    }
}