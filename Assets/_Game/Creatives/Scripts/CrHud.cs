using _Game.UI._Hud;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;

namespace Assets._Game.Creatives.Scripts
{
    public class CrHud : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;
        [SerializeField] private CoinCounterView _counterView;

        public CoinCounterView CounterView => _counterView;

        private IPauseManager _pauseManager;
        private IAudioService _audioService;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAudioService audioService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _audioService = audioService;

            Show();
        }

        public void Show()
        {
            _canvas.enabled = true;
            _counterView.Clear();
            UnsubscribePauseToggle();
            SubscribePauseToggle();
        }
        
        private void SubscribePauseToggle()
        {
            _pauseToggle.ValueChanged += OnPauseClicked;
        }

        private void UnsubscribePauseToggle() => 
            _pauseToggle.ValueChanged -= OnPauseClicked;

        public void Hide() => 
            _canvas.enabled = false;

        public void OnCoinsChanged(float amount) => 
            _counterView.UpdateCoins(amount);
        
        private void OnPauseClicked(bool isPaused)
        {
            _audioService.PlayButtonSound();
            _pauseManager.SetPaused(isPaused);
        }
    }
}