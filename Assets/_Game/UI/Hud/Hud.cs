using System;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI.Common.Scripts;
using _Game.Utils.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Hud
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _quitButton;
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;

        public event Action QuitGame;
        
        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private IAudioService _audioService;
        
        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _audioService = audioService;

            Hide();

            //_quitButton.onClick.AddListener(OnQuitButtonClicked);
            //_pauseToggle.ValueChanged += OnPauseClicked;
        }

        public void Show()
        {
            _canvas.enabled = true;
        }
        
        public void Hide()
        {
            _canvas.enabled = false;
        }
        
        private void OnPauseClicked(bool isPaused)
        {
            _audioService.PlayButtonSound();
            
            _pauseManager.SetPaused(isPaused);
        }

        private async void OnQuitButtonClicked()
        {
            _audioService.PlayButtonSound();
            
            OnPauseClicked(true);
            var popup = await _alertPopupProvider.Load();
             var isConfirmed = await popup.Value.AwaitForDecision("Are you sure to quit?");
            OnPauseClicked(false);
             if (isConfirmed)
                 QuitGame?.Invoke();
             popup.Dispose();
        }

        private void OnDestroy()
        {
            _pauseToggle.ValueChanged -= OnPauseClicked;
        }
        
    }
}