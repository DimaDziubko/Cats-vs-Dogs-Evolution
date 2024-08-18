using System;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Header.Scripts;
using _Game.Utils;
using Assets._Game.Core.Services.Audio;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.Settings.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public class StartBattleScreen : MonoBehaviour, IUIScreen
    {
        public event Action Opened;
        public GameScreen GameScreen => GameScreen.Battle;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _startBattleButton;
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _previousBattleButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private CheatPanel _cheatPanel;
        
        private IAudioService _audioService;

        private IBattleManager _battleManager;
        private IHeader _header;
        private IMyLogger _logger;
        private ISettingsPopupProvider _settingsPopupProvider;
        private IBattleNavigator _battleNavigator;


        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IHeader header,
            IBattleManager battleManager,
            IMyLogger logger,
            IUserContainer userContainer,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            ITimelineConfigRepository timelineConfigRepository)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;

            _battleManager = battleManager;

            _header = header;
            _logger = logger;
            
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
            
            _cheatPanel.Construct(
                timelineNavigator, 
                ageNavigator, 
                battleNavigator, 
                userContainer, 
                timelineConfigRepository, 
                audioService);
        }

        public void Show()
        {
            ShowName();

            Unsubscribe();
            Subscribe();

            _cheatPanel.Init();
            _canvas.enabled = true;
            
            Opened?.Invoke();
        }

        private void ShowName()
        {
            var fullName = $"{GameScreen} {_battleNavigator.CurrentBattle + 1}";
            _header.ShowWindowName(fullName, Color.white);
        }

        public void Hide()
        {
            Unsubscribe();
            _cheatPanel.Cleanup();
            _canvas.enabled = false;
        }

        private void Subscribe()
        {
            _battleNavigator.NavigationUpdated += UpdateNavigationButtons;
            Opened += _battleNavigator.OnStartBattleWindowOpened;
            
            _previousBattleButton.onClick.AddListener(OnPreviousBattleButtonClick);
            _nextBattleButton.onClick.AddListener(OnNextBattleButtonClick);
            _startBattleButton.onClick.AddListener(OnStartButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsBtnClick);
            _quitButton.onClick.AddListener(OnQuitBtnClick);
        }

        private void Unsubscribe()
        {
            _startBattleButton.onClick.RemoveAllListeners();
            _nextBattleButton.onClick.RemoveAllListeners();
            _previousBattleButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();

            _battleNavigator.NavigationUpdated -= UpdateNavigationButtons;
            Opened -= _battleNavigator.OnStartBattleWindowOpened;
        }
        
        private void OnQuitBtnClick()
        {
            PlayButtonSound();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_ANDROID || UNITY_STANDALONE || UNITY_IOS
            Application.Quit();
#endif
        }

        private async void OnSettingsBtnClick()
        {
            PlayButtonSound();
            var popup = await _settingsPopupProvider.Load();
            await popup.Value.AwaitForExit();
            popup.Dispose();
        }

        private void UpdateNavigationButtons(BattleNavigationModel model)
        {
            _previousBattleButton.gameObject.SetActive(model.CanMoveToPreviousBattle);
            _nextBattleButton.gameObject.SetActive(model.CanMoveToNextBattle);
            _startBattleButton.interactable = model.IsPrepared;
            
            ShowName();
        }

        private void OnStartButtonClick()
        {
            PlayButtonSound();
            _battleManager.StartBattle();
        }

        private void OnPreviousBattleButtonClick()
        {
            PlayButtonSound();
            _battleNavigator.MoveToPreviousBattle();
        }

        private void OnNextBattleButtonClick()
        {
            PlayButtonSound();
            _battleNavigator.MoveToNextBattle();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}
