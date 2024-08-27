using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using UnityEngine;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private ToggleButton _cardsBtn;
        [SerializeField] private ToggleButton _skillsBtn;
        [SerializeField] private ToggleButton _runesBtn;
        [SerializeField] private ToggleButton _heroesBtn;

        private LocalStateMachine _localStateMachine;
        private ToggleButton _activeBtn;
        
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IAudioService _audioService;

        private bool IsCardsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_cardsBtn);
        
        public void Construct(
            IWorldCameraService cameraService,
            ICardsScreenProvider provider, 
            IUINotifier uiNotifier,
            IFeatureUnlockSystem featureUnlockSystem,
            IAudioService audioService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _featureUnlockSystem = featureUnlockSystem;
            _audioService = audioService;
            
            _localStateMachine = new LocalStateMachine();
            CardsState cardsState = new  CardsState(this, provider, _cardsBtn, uiNotifier);

            _localStateMachine.AddState(cardsState);
            
            _canvas.enabled = false;
        }

        public void Show()
        {
            _cardsBtn.Initialize(IsCardsUnlocked, OnCardsBtnClicked, PlayButtonSound);
            OnCardsBtnClicked(_cardsBtn);
            _canvas.enabled = true;
        }

        private void OnCardsBtnClicked(ToggleButton button) => 
            _localStateMachine.Enter<CardsState>();

        public void Hide()
        {
            _canvas.enabled = false;
            _cardsBtn.Cleanup();
            _localStateMachine.Cleanup();
        }

        public void SetActiveButton(ToggleButton button)
        {
            _activeBtn = button;
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}