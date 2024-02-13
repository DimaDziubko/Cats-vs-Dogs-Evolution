using System;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Gameplay.Food.Scripts;
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
        [SerializeField] private FoodPanel _foodPanel;
        
        public event Action QuitGame;
        
        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private IAudioService _audioService;
        private IBattleStateService _battleState;
        private IAgeStateService _ageState;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IBattleStateService battleState,
            IAgeStateService ageState)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _audioService = audioService;
            _battleState = battleState;
            _ageState =  ageState;
            
            Hide();

            //_quitButton.onClick.AddListener(OnQuitButtonClicked);
            //_pauseToggle.ValueChanged += OnPauseClicked;
        }

        public void Show()
        {
            _canvas.enabled = true;
            _foodPanel.SetupIcon(_ageState.GetCurrentFoodIcon);
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

        public void UpdateFoodFillAmount(float progress)
        {
            _foodPanel.UpdateFillAmount(progress);
        }

        public void OnFoodChanged(int amount)
        {
            _foodPanel.OnFoodChanged(amount);
        }
    }
}