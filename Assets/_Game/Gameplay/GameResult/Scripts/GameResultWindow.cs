using System;
using System.Threading.Tasks;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
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
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private GameResultIntroAnimation _introAnimation;
        [SerializeField] private Button _quitButton;
        
        [SerializeField] private Canvas _canvas;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IMyLogger _logger;
        private IAudioService _audioService;

        public void Construct(
            Camera uiCamera, 
            IAudioService audioService, 
            IMyLogger logger)
        {
            _audioService = audioService;
            _logger = logger;
            _canvas.worldCamera = uiCamera;
            _quitButton.onClick.AddListener(OnQuitClicked);
        }

        public async UniTask<bool> ShowAndAwaitForExit(float collectedCoins, GameResultType result)
        {
            _coinsLabel.text = collectedCoins.FormatMoney();
            
            _taskCompletion = new UniTaskCompletionSource<bool>();
            
            _quitButton.interactable = false;
            
            _canvas.enabled = true;
            await _introAnimation.Play(result);

            _quitButton.interactable = true;
            
            var isExit = await _taskCompletion.Task;
            return isExit;
        }
        
        private void OnQuitClicked()
        {
            _taskCompletion.TrySetResult(true);
            _canvas.enabled = false;
        }
    }
}