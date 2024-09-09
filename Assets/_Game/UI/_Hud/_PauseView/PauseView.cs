using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._AlertPopup;
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
        private IAlertPopupProvider _alertPopupProvider;
        private IBattleManager _battleManager;
        private Hud _hud;

        public void Construct(
            IAudioService audioService,
            IFeatureUnlockSystem featureUnlockSystem,
            IAlertPopupProvider alertPopupProvider,
            IBattleManager battleManager,
            Hud hud)
        {
            _audioService = audioService;
            _featureUnlockSystem = featureUnlockSystem;
            _alertPopupProvider = alertPopupProvider;
            _battleManager = battleManager;
            _hud = hud;
            
            Hide();
        }

        public void Init()
        {
            SubscribePauseToggle();
        }

        public void Cleanup()
        {
            UnsubscribePauseToggle();
        }
        
        public void Show()
        {
            var isPauseAvailable = _featureUnlockSystem.IsFeatureUnlocked(_pauseToggle);
            _pauseToggle.SetActive(isPauseAvailable);
        }

        private void SubscribePauseToggle() => 
            _pauseToggle.ValueChanged += OnPauseClicked;

        public void Hide() => _pauseToggle.SetActive(false);

        private void UnsubscribePauseToggle() => 
            _pauseToggle.ValueChanged -= OnPauseClicked;


        private void OnPauseClicked(bool isPaused)
        {
            _audioService.PlayButtonSound();
            _battleManager.SetPaused(isPaused);
            ShowAlertPopup();
        }

        private async void ShowAlertPopup()
        {
            var popup = await _alertPopupProvider.Load();
            var isConfirmed = await popup.Value.AwaitForDecision("End battle?");
            
            _pauseToggle.UpdateToggleStateManually(false);
            if (isConfirmed) _hud.Quit();
            _battleManager.SetPaused(false);
            popup.Value.Cleanup();
            popup.Dispose();
        }
        
    }
}