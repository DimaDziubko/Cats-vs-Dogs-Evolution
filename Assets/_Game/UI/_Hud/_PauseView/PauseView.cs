using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Popups;
using UnityEngine;

namespace _Game.UI._Hud._PauseView
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;
        
        private IAudioService _audioService;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private Hud _hud;

        public void Construct(
            IAudioService audioService,
            IFeatureUnlockSystem featureUnlockSystem,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            Hud hud)
        {
            _audioService = audioService;
            _featureUnlockSystem = featureUnlockSystem;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _hud = hud;
            
            HidePauseToggle();
        }
        
        public void ShowPauseToggle()
        {
            SubscribePauseToggle();
            var isPauseAvailable = _featureUnlockSystem.IsFeatureUnlocked(_pauseToggle);
            _pauseToggle.SetActive(isPauseAvailable);
        }

        private void SubscribePauseToggle()
        {
            _pauseToggle.ValueChanged += OnPauseClicked;
        }

        public void HidePauseToggle()
        {
            UnsubscribePauseToggle();
            _pauseToggle.SetActive(false);
        }

        private void UnsubscribePauseToggle() => 
            _pauseToggle.ValueChanged -= OnPauseClicked;


        private void OnPauseClicked(bool isPaused)
        {
            _audioService.PlayButtonSound();
            _pauseManager.SetPaused(isPaused);
            ShowAlertPopup();
        }

        private async void ShowAlertPopup()
        {
            var popup = await _alertPopupProvider.Load();
            var isConfirmed = await popup.Value.AwaitForDecision("End battle?");
            
            _pauseToggle.UpdateToggleStateManually(false);
            
            if (isConfirmed)
            {
                _hud.Quit();
            }
            else
            {
                _pauseManager.SetPaused(false);
            }
            
            popup.Dispose();
        }
    }
}