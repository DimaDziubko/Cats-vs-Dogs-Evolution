using System;
using _Game.Core.Services.Camera;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.GameResult.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class GameResultWindow : MonoBehaviour
    {
        [SerializeField] private GameResultIntroAnimation _introAnimation;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _quitButton;
        
        [SerializeField] private TMP_Text _nextButtonLabel;

        private const string NEXT_LEVEL = "Next";
        private const string RESTART_LEVEL = "Restart";
        
        private Canvas _canvas;
        
        private Action _onNext;
        private Action _onQuit;

        public async void Show(int award, GameResultType type, Action onNext, Action onQuit)
        {
            _onNext = onNext;
            _onQuit = onQuit;

            _nextButton.interactable = false;
            _quitButton.interactable = false;

            AdjustNextButtonLabel(type);
            
            _canvas.enabled = true;
            await _introAnimation.Play(award, type);
            
            _nextButton.interactable = true;
            _quitButton.interactable = true;
        }

        private void AdjustNextButtonLabel(GameResultType type)
        {
            switch (type)
            {
                case GameResultType.Victory:
                    _nextButtonLabel.text = NEXT_LEVEL;
                    break;
                case GameResultType.Defeat:
                    _nextButtonLabel.text = RESTART_LEVEL;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void Construct(IWorldCameraService cameraService)
        {
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _canvas.enabled = false;
            _nextButton.onClick.AddListener(OnNextClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnNextClicked()
        {
            _onNext?.Invoke();
            _canvas.enabled = false;
        }

        private void OnQuitClicked()
        { ;
            _onQuit?.Invoke();
            _canvas.enabled = false;
        }
    }
}
