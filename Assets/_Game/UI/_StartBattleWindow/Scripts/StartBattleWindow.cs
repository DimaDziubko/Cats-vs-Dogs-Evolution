using System;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Gameplay.BattleLauncher;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public class StartBattleWindow : MonoBehaviour, IUIWindow
    {
        public event Action Opened;
        
        public string Name => $"Battle {_battleState.CurrentBattleIndex + 1}";

        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _startBattleButton;
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _previousBattleButton;

        [SerializeField] private AudioClip _startBattleSound;
        
        private IAudioService _audioService;

        private IBattleLaunchManager _battleLaunchManager;
        private IBattleStateService _battleState;
        private IHeader _header;
        private IMyLogger _logger;
        
        public void Construct(
            Camera uICamera,
            IAudioService audioService,

            IHeader header,
            IBattleLaunchManager battleLaunchManager,

            IBattleStateService battleState,
            IMyLogger logger)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;

            _battleLaunchManager = battleLaunchManager;

            _header = header;

            _battleState = battleState;
            _logger = logger;
        }

        public void Show()
        {
            _header.ShowWindowName(Name);
            
            Unsubscribe();
            Subscribe();

            _canvas.enabled = true;
            
            Opened?.Invoke();
        }

        public void Hide()
        {
            //TODO Delete
            _logger.Log("Start battle window HIDE");
            
            Unsubscribe();

            _canvas.enabled = false;
        }

        private void Unsubscribe()
        {
            _startBattleButton.onClick.RemoveAllListeners();
            _nextBattleButton.onClick.RemoveAllListeners();
            _previousBattleButton.onClick.RemoveAllListeners();

            _battleState.NavigationUpdated -= UpdateNavigationButtons;
            Opened -= _battleState.OnStartBattleWindowOpened;
        }

        private void Subscribe()
        {
            _battleState.NavigationUpdated += UpdateNavigationButtons;
            Opened += _battleState.OnStartBattleWindowOpened;

            _previousBattleButton.onClick.AddListener(OnPreviousBattleButtonClick);
            _nextBattleButton.onClick.AddListener(OnNextBattleButtonClick);
            _startBattleButton.onClick.AddListener(OnStartButtonClick);
        }


        private void UpdateNavigationButtons(BattleNavigationModel model)
        {
            _previousBattleButton.gameObject.SetActive(model.IsFirstBattle);
            _nextBattleButton.gameObject.SetActive(!model.IsLastBattle);
            _previousBattleButton.interactable = model.CanMoveToPreviousBattle;
            _nextBattleButton.interactable = model.CanMoveToNextBattle;

            _startBattleButton.interactable = model.IsPrepared;
            _header.ShowWindowName(Name);
        }
        
        private void OnStartButtonClick()
        {
            PlayButtonSound();
            _battleLaunchManager.TriggerLaunchBattle();
            
            if (_startBattleSound != null)
            {
                _audioService.PlayOneShot(_startBattleSound);
            }
        }

        private void OnPreviousBattleButtonClick()
        {
            PlayButtonSound();
            DisableButtons();
            _battleState.MoveToPreviousBattle();
        }

        private void OnNextBattleButtonClick()
        {
            PlayButtonSound();
            DisableButtons();
            _battleState.MoveToNextBattle();
        }

        private void DisableButtons()
        {
            _nextBattleButton.interactable = false;
            _previousBattleButton.interactable = false;
            _startBattleButton.interactable = false;
        }
        
        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}
